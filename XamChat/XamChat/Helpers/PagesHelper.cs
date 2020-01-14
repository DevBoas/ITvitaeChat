using ITvitaeChat2.View;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITvitaeChat2.Helpers
{
    /// <summary>
    /// Class containing ONE instance of pages that don't need more than one
    /// </summary>
    public class PagesHelper : ObservableObject
    {
        // Default constructor
        public PagesHelper()
        {

        }

        // Home
        private HomePage homePage;
        public HomePage pHomePage
        {
            get
            {
                if (homePage == null)
                {
                    homePage = new HomePage();
                }
                return homePage;
            }
            set => SetProperty(ref homePage, value);
        }

        // Login
        private LoginPage loginPage;
        public LoginPage pLoginPage
        {
            get
            {
                if (loginPage == null)
                {
                    loginPage = new LoginPage();
                }
                return loginPage;
            }
            set => SetProperty(ref loginPage, value);
        }

        // Profile
        private ProfilePage profilePage;
        public ProfilePage pProfilePage
        {
            get
            {
                if (profilePage == null)
                {
                    profilePage = new ProfilePage();
                }
                return profilePage;
            }
            set => SetProperty(ref profilePage, value);
        }
    }
}
