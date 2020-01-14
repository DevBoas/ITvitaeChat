using MvvmHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using ITvitaeChat2.Helpers;
using ITvitaeChat2.View;

namespace ITvitaeChat2.ViewModel
{
    public class LobbyViewModel : ViewModelBase
    {
        public List<string> Rooms { get; }
        public LobbyViewModel()
        {
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
        
    }
}
