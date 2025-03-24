using MauiApp2.Pages;

namespace MauiApp2;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("registerpage", typeof(RegisterPage));
		Routing.RegisterRoute(nameof(ShowObjectsPage), typeof(ShowObjectsPage));
		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(AddSubPage), typeof(AddSubPage));
		Routing.RegisterRoute(nameof(DeleteSubPage), typeof(DeleteSubPage));

		Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
		


		
	}
}
