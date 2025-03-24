namespace MauiApp2.Pages;
using MauiApp2.ViewModel;

public partial class ShowObjectsPage : ContentPage
{
	public ShowObjectsPage()
	{
		InitializeComponent();
		BindingContext = new ShowObjectsViewModel();
	}
}