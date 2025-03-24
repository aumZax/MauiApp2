using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using MauiApp2.Pages;
using MauiApp2.Services; // เพิ่ม namespace สำหรับ UserService
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace MauiApp2.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        string username = "";

        [ObservableProperty]
        string password = "";

        [RelayCommand]
        async Task Login()
        {
            // ตรวจสอบว่าชื่อผู้ใช้และรหัสผ่านไม่ว่างเปล่า
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Username and password are required.", "OK");
                return;
            }

            // โหลดข้อมูลจากไฟล์ JSON
            var users = await LoadUsersFromJson();

            // ตรวจสอบข้อมูล
            var user = users.FirstOrDefault(u => u.Email == username && u.Password == password);

            if (user != null)
            {
                // ถ้าข้อมูลถูกต้อง
                System.Diagnostics.Debug.WriteLine(user.Uid);

                // บันทึก UID ผู้ใช้ที่ล็อกอินลงใน Preferences
                Preferences.Set("LoggedInUserId", user.Uid);

                // อัปเดตข้อมูลผู้ใช้ใน UserService
                UserService.Instance.LoggedInUser = user;

                // ไปยังหน้า ShowObjectsPage
                await GotoPage(nameof(ShowObjectsPage));
            }
            else
            {
                // ถ้าข้อมูลไม่ถูกต้อง
                await App.Current.MainPage.DisplayAlert("Error", "Invalid username or password", "OK");
            }
        }

        [RelayCommand]
        async Task GotoPage(string page)
        {
            await Shell.Current.GoToAsync(page);
        }

        // โหลดข้อมูลจากไฟล์ JSON
        private async Task<List<Userr>> LoadUsersFromJson()
        {
            try
            {
                // อ่านไฟล์ JSON จากแอสเซมบลี
                var stream = await FileSystem.OpenAppPackageFileAsync("usersx.json");
                using (var reader = new StreamReader(stream))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<List<Userr>>(json)!;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Failed to load user data: {ex.Message}", "OK");
                return new List<Userr>(); // คืนค่า List ว่างแทน null
            }
        }

        // Command สำหรับการล็อกเอาท์
        [RelayCommand]
        async Task Logout()
        {
            // ลบ UID ผู้ใช้ที่ล็อกอินออกจาก Preferences
            Preferences.Remove("LoggedInUserId");

            // ล้างข้อมูลผู้ใช้ใน UserService
            UserService.Instance.LoggedInUser = null;

            // กลับไปยังหน้า Login
            await GotoPage(nameof(LoginPage));
        }
    }
}