using MauiApp2.ViewModel;

namespace MauiApp2.Pages;

public partial class DeleteSubPage : ContentPage
{
	public DeleteSubPage()
	{
		InitializeComponent();
		BindingContext = new DeleteSubViewModel();

	}
}