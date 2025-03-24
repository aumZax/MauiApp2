using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using MauiApp2.Pages;
using MauiApp2.Services;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp2.ViewModel
{
    public partial class ShowObjectsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<CurrentTermRegistration> currentTermCourses = new ObservableCollection<CurrentTermRegistration>();

        [ObservableProperty]
        private string currentTermTitle = "Current Term Registration";

        [ObservableProperty]
        private Userr? loggedInUser;

        public ShowObjectsViewModel()
        {
            LoadDataAsync();
            SubscribeToMessages(); // สมัครรับข้อความ
        }

        private void SubscribeToMessages()
        {
            // สมัครรับข้อความจาก DeleteSubViewModel
            MessagingCenter.Subscribe<DeleteSubViewModel, Userr>(this, "UserDataUpdated", (sender, updatedUser) =>
            {
                if (updatedUser != null)
                {
                    LoggedInUser = updatedUser; // อัปเดตข้อมูลผู้ใช้
                    SwitchTerm("0"); // อัปเดตข้อมูลในหน้า
                }
                else
                {
                    Debug.WriteLine("Updated user is null");
                }
            });

            // สมัครรับข้อความจาก AddSubViewModel
            MessagingCenter.Subscribe<AddSubViewModel, Userr>(this, "UserDataUpdated", (sender, updatedUser) =>
            {
                if (updatedUser != null)
                {
                    LoggedInUser = updatedUser; // อัปเดตข้อมูลผู้ใช้
                    SwitchTerm("0"); // อัปเดตข้อมูลในหน้า
                }
                else
                {
                    Debug.WriteLine("Updated user is null");
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
                    SwitchTerm("0");
                }
                else
                {
                    Debug.WriteLine("User not found.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "Failed to load user data.", "OK");
            }
        }

        [RelayCommand]
        void SwitchTerm(string term)
        {
            try
            {
                CurrentTermCourses.Clear();

                if (LoggedInUser == null)
                {
                    Debug.WriteLine("LoggedInUser is null");
                    return;
                }

                if (term == "0")
                {
                    CurrentTermTitle = "Current Term Registration";
                    if (LoggedInUser.CurrentTermRegistration != null)
                    {
                        foreach (var course in LoggedInUser.CurrentTermRegistration)
                        {
                            CurrentTermCourses.Add(course);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("CurrentTermRegistration is null");
                    }
                }
                else
                {
                    var selectedTerm = LoggedInUser.PreviousTermsRegistration?.FirstOrDefault(t => t.Term == term);
                    if (selectedTerm != null && selectedTerm.Courses != null)
                    {
                        CurrentTermTitle = $"Term {term} Registration";
                        foreach (var course in selectedTerm.Courses)
                        {
                            CurrentTermCourses.Add(course);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Selected term not found or courses are null");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SwitchTerm: {ex.Message}");
            }
        }

        [RelayCommand]
        async Task GoToAddSubPage()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(AddSubPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to AddSubPage: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "Failed to navigate to AddSubPage.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToProfilePage()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(ProfilePage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to ProfilePage: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "Failed to navigate to ProfilePage.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToDeleteSubPage()
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(DeleteSubPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to DeleteSubPage: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "Failed to navigate to DeleteSubPage.", "OK");
            }
        }
    }
}