using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using MauiApp2.Pages;
using MauiApp2.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp2.ViewModel
{
    public partial class ShowObjectsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<CurrentTermRegistration> currentTermCourses = new();

        [ObservableProperty]
        private string currentTermTitle = "Current Term Registration";

        [ObservableProperty]
        private Userr? loggedInUser;

        [ObservableProperty]
        private ObservableCollection<string> academicYears = new() { "1", "2" };

        [ObservableProperty]
        private string selectedAcademicYear = "2"; // ตั้งค่าเริ่มต้นเป็นปี 2
[ObservableProperty]
private bool showAddDeleteButtons = true; // เพิ่ม Property นี้
        public ShowObjectsViewModel()
        {
            LoadDataAsync();
            SubscribeToMessages();
        }

        private void SubscribeToMessages()
        {
            MessagingCenter.Subscribe<DeleteSubViewModel, Userr>(this, "UserDataUpdated", (sender, updatedUser) =>
            {
                if (updatedUser != null)
                {
                    LoggedInUser = updatedUser;
                    SwitchTerm("3"); // เทอมปัจจุบันคือเทอม 3
                }
            });

            MessagingCenter.Subscribe<AddSubViewModel, Userr>(this, "UserDataUpdated", (sender, updatedUser) =>
            {
                if (updatedUser != null)
                {
                    LoggedInUser = updatedUser;
                    SwitchTerm("3"); // เทอมปัจจุบันคือเทอม 3
                }
            });
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var loggedInUser = UserService.Instance.LoggedInUser;
                if (loggedInUser != null)
                {
                    LoggedInUser = loggedInUser;
                    SwitchTerm("3"); // โหลดเทอมปัจจุบัน (เทอม 3) โดย default
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        [RelayCommand]
void SwitchTerm(string termNumber)
{
    try
    {
        CurrentTermCourses.Clear();

        if (LoggedInUser == null) return;

        string term;
        if (termNumber == "3" && selectedAcademicYear == "2")
        {
            // เทอมปัจจุบัน (ปี 2 เทอม 3)
            term = "Current Term";
            CurrentTermTitle = $"ปี {selectedAcademicYear} - เทอม {termNumber}";
            
            if (LoggedInUser.CurrentTermRegistration != null)
            {
                foreach (var course in LoggedInUser.CurrentTermRegistration)
                {
                    CurrentTermCourses.Add(course);
                }
            }
        }
        else
        {
            // เทอมอื่นๆ
            int yearOffset = selectedAcademicYear == "1" ? 2 : 3; // ปี 1 = 2022, ปี 2 = 2023
            term = $"{termNumber}/202{yearOffset}";
            CurrentTermTitle = $"ปี {selectedAcademicYear} - เทอม {termNumber}";

            var selectedTerm = LoggedInUser.PreviousTermsRegistration?
                .FirstOrDefault(t => t.Term == term);

            if (selectedTerm != null)
            {
                foreach (var course in selectedTerm.Courses)
                {
                    CurrentTermCourses.Add(new CurrentTermRegistration
                    {
                        CourseId = course.CourseId,
                        CourseName = course.CourseName,
                        Credit = course.Credit,
                        Grade = course.Grade
                    });
                }
            }
            else
            {
                Debug.WriteLine($"Term {term} not found in previous terms");
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Error in SwitchTerm: {ex.Message}");
    }
}
partial void OnSelectedAcademicYearChanged(string value)
{
    // ซ่อนปุ่มเพิ่ม/ลดวิชาเมื่อเลือกปี 1
    ShowAddDeleteButtons = value != "1";
    
    // รีโหลดข้อมูลโดยเรียก SwitchTerm ใหม่
    if (value == "1")
    {
        SwitchTerm("1"); // โหลดเทอม 1 ของปี 1
    }
    else
    {
        SwitchTerm("3"); // โหลดเทอม 3 ของปี 2 (เทอมปัจจุบัน)
    }
}

        [RelayCommand]
        async Task GoToAddSubPage() => await Shell.Current.GoToAsync(nameof(AddSubPage));

        [RelayCommand]
        async Task GoToProfilePage() => await Shell.Current.GoToAsync(nameof(ProfilePage));

        [RelayCommand]
        async Task GoToDeleteSubPage() => await Shell.Current.GoToAsync(nameof(DeleteSubPage));
    }
}