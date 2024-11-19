using Module07Data_Access.ViewModel;

namespace Module07Data_Access.View;

public partial class ViewPersonal : ContentPage
{
    public ViewPersonal()
    {
        InitializeComponent();
        BindingContext = new PersonalViewModel();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        // Navigate back to MainPage
        await Shell.Current.GoToAsync("//MainPage");
    }
}
