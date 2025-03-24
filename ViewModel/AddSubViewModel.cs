using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using MauiApp2.Model2;
using MauiApp2.Pages;
using MauiApp2.Services;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MauiApp2.ViewModel;

public partial class AddSubViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Courses> availableCourses = new ObservableCollection<Courses>();

    [ObservableProperty]
    private ObservableCollection<Courses> filteredCourses = new ObservableCollection<Courses>();

    [ObservableProperty]
    private ObservableCollection<Courses> selectedCourses = new ObservableCollection<Courses>();

    [ObservableProperty]
    private string loggedInUserName;

    [ObservableProperty]
    private string searchQuery = "";

    public AddSubViewModel()
    {
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            // ล้างรายวิชาที่เลือก
            SelectedCourses.Clear();

            // อ่านข้อมูลผู้ใช้จาก UserService
            var loggedInUser = UserService.Instance.LoggedInUser;

            if (loggedInUser != null)
            {
                // บันทึกชื่อผู้ใช้ที่ล็อกอิน
                LoggedInUserName = loggedInUser.Profile.Name;

                // โหลดรายวิชาทั้งหมดจากไฟล์ JSON
                var allCourses = await ReadCoursesJsonAsync();

                // กรองเฉพาะวิชาที่ยังไม่เคยลงทะเบียน
                var availableCourses = allCourses
                    .Where(course => !loggedInUser.CurrentTermRegistration
                        .Any(registeredCourse => registeredCourse.CourseId == course.CourseId))
                    .ToList();

                // ล้างรายวิชาเก่าก่อนเพิ่มใหม่
                AvailableCourses.Clear();

                // เพิ่มรายวิชาที่ยังไม่ได้ลงทะเบียน
                foreach (var course in availableCourses)
                {
                    AvailableCourses.Add(course);
                }

                // กรองรายวิชาตามคำค้นหา (ถ้ามี)
                FilterCourses();
            }
            else
            {
                Debug.WriteLine("User not found.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading data: {ex.Message}");
            await App.Current.MainPage.DisplayAlert("Error", "Failed to load data.", "OK");
        }
    }

    private async Task<List<Courses>> ReadCoursesJsonAsync()
    {
        try
        {
            // อ่านไฟล์ JSON จากแอสเซมบลี
            using var stream = await FileSystem.OpenAppPackageFileAsync("courses.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();
            return Courses.FromJson(contents);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reading JSON: {ex.Message}");
            throw; // ส่งข้อผิดพลาดไปยัง caller
        }
    }

    // Command สำหรับเลือกหรือยกเลิกรายวิชา
    [RelayCommand]
    void SelectCourse(Courses course)
    {
        if (SelectedCourses.Contains(course))
        {
            // ถ้าวิชาถูกเลือกอยู่แล้ว ให้เอาออก
            SelectedCourses.Remove(course);
        }
        else
        {
            // ถ้าวิชายังไม่ถูกเลือก ให้เพิ่มเข้าไป
            SelectedCourses.Add(course);
        }
    }

    // Command สำหรับยืนยันการเลือกรายวิชา
    [RelayCommand]
    async Task ConfirmSelection()
    {
        try
        {
            // อ่านข้อมูลผู้ใช้จาก UserService
            var loggedInUser = UserService.Instance.LoggedInUser;

            if (loggedInUser != null)
            {
                // เพิ่มวิชาที่เลือกทั้งหมดลงในเทอมปัจจุบัน
                foreach (var course in AvailableCourses.Where(c => c.IsSelected))
                {
                    loggedInUser.CurrentTermRegistration.Add(new CurrentTermRegistration
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName
                    });
                }

                // แสดงข้อความสำเร็จ
                await App.Current.MainPage.DisplayAlert("Success", "Courses added successfully.", "OK");

                // ส่งข้อมูลผู้ใช้ที่อัปเดตไปยัง ShowObjectsViewModel
                MessagingCenter.Send(this, "UserDataUpdated", loggedInUser);

                // กลับไปยังหน้า ShowObjectsPage
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error confirming selection: {ex.Message}");
            await App.Current.MainPage.DisplayAlert("Error", "Failed to confirm selection.", "OK");
        }
    }

    // Command สำหรับค้นหารายวิชา
    [RelayCommand]
    void Search()
    {
        FilterCourses();
    }

    // Method สำหรับกรองรายวิชาตามคำค้นหา
    private void FilterCourses()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            // ถ้าไม่มีคำค้นหา แสดงรายวิชาทั้งหมด
            FilteredCourses = new ObservableCollection<Courses>(AvailableCourses);
        }
        else
        {
            // กรองรายวิชาตามรหัสวิชา (CourseId)
            var filtered = AvailableCourses
                .Where(course => course.CourseId.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
            FilteredCourses = new ObservableCollection<Courses>(filtered);
        }
    }

    // Command สำหรับกลับไปยังหน้าก่อนหน้า
    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}