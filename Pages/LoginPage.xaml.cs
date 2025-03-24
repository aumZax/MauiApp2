namespace MauiApp2.Pages;
using MauiApp2.ViewModel;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = new LoginViewModel();
	}


    private async void RegisterPage(object sender, TappedEventArgs e)
    {
		await Shell.Current.GoToAsync("registerpage");
    }
}