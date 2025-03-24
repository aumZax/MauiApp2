using MauiApp2.ViewModel;

namespace MauiApp2.Pages;

public partial class AddSubPage : ContentPage
{
	public AddSubPage()
	{
		InitializeComponent();
		BindingContext = new AddSubViewModel();

	}
}