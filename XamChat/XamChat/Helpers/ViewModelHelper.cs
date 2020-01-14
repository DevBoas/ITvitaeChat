using ITvitaeChat2.ViewModel;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITvitaeChat2.Helpers
{
    /// <summary>
    /// Class containing ONE instance of viewmodels that don't need more than one
    /// </summary>
    public class ViewModelHelper : ObservableObject
    {
        // Default constructor
        public ViewModelHelper()
        {

        }

        // Lobby
        private LobbyViewModel lobbyViewModel;
        public LobbyViewModel pLobbyViewModel
        {
            get
            {
                if (lobbyViewModel == null)
                {
                    lobbyViewModel = new LobbyViewModel();
                }
                return lobbyViewModel;
            }
            set => SetProperty(ref lobbyViewModel, value);
        }

        // Login
        private LoginViewModel loginViewModel;
        public LoginViewModel pLoginViewModel
        {
            get
            {
                if (loginViewModel == null)
                {
                    loginViewModel = new LoginViewModel();
                }
                return loginViewModel;
            }
            set => SetProperty(ref loginViewModel, value);
        }

        // Profile
        private ProfileViewModel profileViewModel;
        public ProfileViewModel pProfileViewModel
        {
            get
            {
                if (profileViewModel == null)
                {
                    profileViewModel = new ProfileViewModel();
                }
                return profileViewModel;
            }
            set => SetProperty(ref profileViewModel, value);
        }
    }
}
