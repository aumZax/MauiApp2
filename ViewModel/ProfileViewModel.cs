using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using MauiApp2.Services;
using MauiApp2.Pages;

namespace MauiApp2.ViewModel;

public partial class ProfileViewModel : ObservableObject
{
    private string userEmail;
    private Profile userProfile;

    public string UserEmail
    {
        get => userEmail;
        set => SetProperty(ref userEmail, value);
    }

    public Profile UserProfile
    {
        get => userProfile;
        set => SetProperty(ref userProfile, value);
    }

    public ProfileViewModel()
    {
        LoadUserDataAsync();
    }

    private async Task LoadUserDataAsync()
    {
        try
        {
            // อ่านข้อมูลผู้ใช้ที่ล็อกอินจาก Preferences
            var loggedInUserId = Preferences.Get("LoggedInUserId", string.Empty);

            if (!string.IsNullOrEmpty(loggedInUserId))
            {
                // โหลดข้อมูลผู้ใช้จากไฟล์ JSON
                var users = await ReadUsersJsonAsync();

                // ค้นหาผู้ใช้ที่ล็อกอินจาก UID
                var loggedInUser = users.FirstOrDefault(u => u.Uid == loggedInUserId);

                if (loggedInUser != null)
                {
                    // บันทึกข้อมูลอีเมลและโปรไฟล์
                    UserEmail = loggedInUser.Email;
                    UserProfile = loggedInUser.Profile;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading user data: {ex.Message}");
            await App.Current.MainPage.DisplayAlert("Error", "Failed to load user data.", "OK");
        }
    }

    private async Task<List<Userr>> ReadUsersJsonAsync()
    {
        try
        {
            // ใช้เส้นทางใน FileSystem.AppDataDirectory สำหรับอ่านไฟล์
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "usersx.json");

            // ถ้าไฟล์ไม่มีอยู่ ให้โหลดจากแอสเซมบลี
            if (!File.Exists(filePath))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("usersx.json");
                using var reader = new StreamReader(stream);
                var contents = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<Userr>>(contents)!;
            }

            // อ่านไฟล์จาก FileSystem.AppDataDirectory
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<List<Userr>>(json)!;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reading JSON: {ex.Message}");
            throw; // ส่งข้อผิดพลาดไปยัง caller
        }
    }
    [RelayCommand]
    async Task Logout()
    {
        try
        {
            Preferences.Remove("LoggedInUserId"); // ลบ UID ที่ล็อกอิน
            UserService.Instance.LoggedInUser = null; // เคลียร์ข้อมูลผู้ใช้
            await Shell.Current.GoToAsync(nameof(LoginPage)); // กลับไปยังหน้า Login
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during logout: {ex.Message}");
            await App.Current.MainPage.DisplayAlert("Error", "Failed to logout.", "OK");
        }
    }

    // Command สำหรับกลับไปยังหน้าก่อนหน้า
    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}