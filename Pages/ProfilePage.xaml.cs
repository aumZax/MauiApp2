using MauiApp2.ViewModel;

namespace MauiApp2.Pages;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		BindingContext = new ProfileViewModel();

	}
}