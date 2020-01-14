using Xamarin.Forms;
using ITvitaeChat2.ViewModel;

namespace ITvitaeChat2.View
{
    public partial class LobbyPage : ContentPage
    {
        public LobbyPage()
        {
            InitializeComponent();

            BindingContext = App.vViewModelHelper.pLobbyViewModel;
        }

        
        async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            await App.vViewModelHelper.pLobbyViewModel.GoToGroupChat(Navigation, e.SelectedItem as string);
            ((ListView)sender).SelectedItem = null;
        }
    }
}
