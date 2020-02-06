using MvvmHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using ITvitaeChat2.Helpers;
using ITvitaeChat2.View;
using System.Windows.Input;

namespace ITvitaeChat2.ViewModel
{
    public class LobbyViewModel : ViewModelBase
    {
        public ICommand FolderClickedCommand { get; }

        public List<string> Rooms { get; }
        public LobbyViewModel()
        {
            FolderClickedCommand = new Command(async () => await GoToFolderPage());
            Rooms = ChatService.GetRooms();
        }

        public string UserName
        {
            get => Settings.UserFirstName;
            set
            {
                if (value == UserName)
                    return;
                Settings.UserFirstName = value;
                OnPropertyChanged();
            }
        }

        public async Task GoToGroupChat(INavigation navigation, string group)
        {
            if (string.IsNullOrWhiteSpace(group))
                return;

            if (string.IsNullOrWhiteSpace(UserName))
                return;

            Settings.Group = group;
            await navigation.PushModalAsync(new ITvitaeChatNavigationPage(new GroupChatPage()));
        }
        
        public async Task GoToFolderPage()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new ITvitaeChatNavigationPage(new FolderIOPage()));
        }
    }
}
