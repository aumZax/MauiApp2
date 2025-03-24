using MauiApp2.Model; // ตรวจสอบว่า namespace นี้ถูกต้อง
namespace MauiApp2.Services
{
    public class UserService
    {
        // Singleton instance
        private static UserService _instance;
        public static UserService Instance => _instance ??= new UserService();

        // Property สำหรับเก็บข้อมูลผู้ใช้ที่ล็อกอิน
        public Userr LoggedInUser { get; set; }

        // Private constructor เพื่อป้องกันการสร้าง instance จากภายนอก
        private UserService() { }
    }
}