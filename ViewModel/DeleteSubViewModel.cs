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

namespace MauiApp2.ViewModel;

public partial class DeleteSubViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<CurrentTermRegistration> registeredCourses = new ObservableCollection<CurrentTermRegistration>();

    [ObservableProperty]
    private ObservableCollection<CurrentTermRegistration> filteredCourses = new ObservableCollection<CurrentTermRegistration>();

    [ObservableProperty]
    private ObservableCollection<CurrentTermRegistration> selectedCourses = new ObservableCollection<CurrentTermRegistration>();

    [ObservableProperty]
    private string loggedInUserName;

    [ObservableProperty]
    private string searchQuery = "";

    public DeleteSubViewModel()
    {
        LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            // อ่านข้อมูลผู้ใช้จาก UserService
            var loggedInUser = UserService.Instance.LoggedInUser;

            if (loggedInUser != null)
            {
                // บันทึกชื่อผู้ใช้ที่ล็อกอิน
                LoggedInUserName = loggedInUser.Profile.Name;

                // โหลดรายวิชาที่ลงทะเบียนในเทอมปัจจุบัน
                RegisteredCourses.Clear();
                foreach (var course in loggedInUser.CurrentTermRegistration)
                {
                    RegisteredCourses.Add(course);
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

    // Command สำหรับเลือกหรือยกเลิกรายวิชา
    [RelayCommand]
    void SelectCourse(CurrentTermRegistration course)
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

  // Command สำหรับลบรายวิชาที่เลือก
[RelayCommand]
async Task DeleteSelectedCourses()
{
    try
    {
        var loggedInUser = UserService.Instance.LoggedInUser;

        if (loggedInUser != null)
        {
            foreach (var course in SelectedCourses.ToList())
            {
                loggedInUser.CurrentTermRegistration.Remove(course);
                RegisteredCourses.Remove(course); // อัปเดต UI
            }

            SelectedCourses.Clear();

            await App.Current.MainPage.DisplayAlert("Success", "Courses deleted successfully.", "OK");

            // ส่งข้อความแจ้งให้ ShowObjectsViewModel อัปเดตข้อมูล
            MessagingCenter.Send(this, "UserDataUpdated", loggedInUser);

            // กลับไปยังหน้าก่อนหน้า
            await Shell.Current.GoToAsync("..");
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error deleting courses: {ex.Message}");
        await App.Current.MainPage.DisplayAlert("Error", "Failed to delete courses.", "OK");
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
            FilteredCourses = new ObservableCollection<CurrentTermRegistration>(RegisteredCourses);
        }
        else
        {
            // กรองรายวิชาตามรหัสวิชา (CourseId)
            var filtered = RegisteredCourses
                .Where(course => course.CourseId.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
            FilteredCourses = new ObservableCollection<CurrentTermRegistration>(filtered);
        }
    }

    // Command สำหรับกลับไปยังหน้าก่อนหน้า
    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}