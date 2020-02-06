using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ITvitaeChat2.Core;
using ITvitaeChat2.View;
using Xamarin.Essentials;
using ITvitaeChat2.Helpers;
using Microsoft.AppCenter.Distribute;
using Microsoft.Identity.Client;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ITvitaeChat2
{
    public partial class App : Application
    {
        #region Variables
        public static IPublicClientApplication PCA = null;
        public static string Username = string.Empty;
        public static ViewModelHelper vViewModelHelper;
        public static PagesHelper vPagesHelper;
        #endregion

        #region Properties
        public static object ParentWindow { get; set; }
        #endregion
        public App()
        {
            InitializeComponent();

            vViewModelHelper = new ViewModelHelper();
            vPagesHelper = new PagesHelper();

            DependencyService.Register<ChatService>();

            MainPage = vPagesHelper.pLoginPage;
        }

        protected override void OnStart()
        {
            // Start AppCenter to help with specific services like chatting --> TODO setup IOS secret
            if (DeviceInfo.Platform == DevicePlatform.Android && Settings.AppCenterAndroid != "AC_ANDROID")
            {
                AppCenter.Start($"android={Settings.AppCenterAndroid};" +
                    "uwp={Your UWP App secret here};" +
                    "ios={Your iOS App secret here}",
                    typeof(Analytics), typeof(Crashes), typeof(Distribute));
            }
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
