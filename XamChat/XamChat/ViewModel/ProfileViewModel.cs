using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using ITvitaeChat2.Helpers;

namespace ITvitaeChat2.ViewModel
{
    public class ProfileViewModel : BaseViewModel
    {

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

        public Uri Avatar
        {
            get => new Uri(Settings.UserPicture);
            set
            {
                if (value == Avatar)
                    return;

                Settings.UserPicture = value.OriginalString;
                OnPropertyChanged();
                
            }
        }

        public string ServerIP
        {
            get => Settings.ServerIP;
            set
            {
                if (value == ServerIP)
                    return;
                Settings.ServerIP = value.ToLower();
                OnPropertyChanged();
            }
        }

    }
}
