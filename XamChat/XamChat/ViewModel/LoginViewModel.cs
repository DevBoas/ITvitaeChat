using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using System.Windows.Input;
using ITvitaeChat2.Helpers;

namespace ITvitaeChat2.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region Variables
        private bool isLoggedIn;
        #endregion

        #region Properties
        private bool pUseBroker { get; set; } = false;
        
        public bool pIsLoggedIn
        {
            get => isLoggedIn;
            set => SetProperty(ref isLoggedIn, value);
        }

        public ICommand pLogInCommand { get; set; }
        #endregion

        // Default constructor
        public LoginViewModel()
        {
            Title = "ITvitae portal";

            pLogInCommand = new Command(async () => await AcquireTokenAsync());

            // Use broker in case of iOS device
            if (Device.RuntimePlatform == Device.iOS) pUseBroker = true;

            CreatePublicClient();
        }

        /// <summary>
        /// Create PCA according to device
        /// </summary>
        public void CreatePublicClient()
        {
            var builder = PublicClientApplicationBuilder
                .Create((string)App.Current.Resources["ClientID"]);

            if (pUseBroker)
            {
                builder.WithBroker();
                builder = builder.WithIosKeychainSecurityGroup("com.microsoft.adalcache");
                builder = builder.WithRedirectUri((string)App.Current.Resources["BrokerRedirectUriOnIos"]);
            }
            else
            {
                builder = builder.WithRedirectUri($"msal{(string)App.Current.Resources["ClientID"]}://auth");
            }

            builder = builder.WithTenantId((string)App.Current.Resources["TenantID"]);

            App.PCA = builder.Build();
        }

        /// <summary>
        /// Updates the users information to the settings class. (In the settings class it will be saved to preferences)
        /// </summary>
        /// <param name="content">Freshly retreived user information</param>
        private void UpdateUserContent(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                JObject user = JObject.Parse(content);

                Settings.UserFirstName = (string)user["givenName"];
                Settings.UserLastName = (string)user["surname"];
                Settings.UserEmail = (string)user["userPrincipalName"];
            }
        }

        /// <summary>
        /// After succesfull retreiving a token we can use this to retreive user information
        /// </summary>
        /// <param name="token">???</param>
        /// <returns>User his information from microsoft</returns>
        private async Task<string> GetHttpContentWithTokenAsync(string token)
        {
            try
            {
                // Get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("API call to graph failed: ", ex.Message, "Dismiss");
                return ex.ToString();
            }
        }

        /// <summary>
        /// If user has recently logged in, loggin silently. Otherwise login normally.
        /// </summary>
        /// <returns>A fresh token related to the user</returns>
        private async Task AcquireTokenAsync()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = true;
                pLoadingMessageTitle = "Logging in";
                pLoadingMessage = "Acquiring acces token silently";
            });
            
            AuthenticationResult authResult = null;
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
            try
            {
                // Try to get token silently in case the user has recently logged in
                try
                {
                    IAccount firstAccount = accounts.FirstOrDefault();
                    authResult = await App.PCA.AcquireTokenSilent((string[])App.Current.Resources["Scopes"], firstAccount).ExecuteAsync();
                }
                // User has not logged in recently so try login normally
                catch (MsalUiRequiredException ex)
                {
                    Device.BeginInvokeOnMainThread(() => { pLoadingMessage = "Failure"; });
                    
                    try
                    {
                        Device.BeginInvokeOnMainThread(() => { pLoadingMessage = "Acquiring acces token"; });

                        authResult = await App.PCA.AcquireTokenInteractive((string[])App.Current.Resources["Scopes"])
                                                    .WithParentActivityOrWindow(App.ParentWindow)
                                                    .WithUseEmbeddedWebView(true)
                                                    .ExecuteAsync();
                    }
                    catch (Exception ex2)
                    {
                        Device.BeginInvokeOnMainThread(() => { IsBusy = false; });
                        await App.Current.MainPage.DisplayAlert("Acquire token interactive failed. See exception message for details: ", ex2.Message, "Dismiss");
                    }
                }

                // If there is any result retreive user information.
                if (authResult != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        pLoadingMessageTitle = "Succes!";
                        pLoadingMessage = "Acces token acquired. Saving user info.";
                        pIsLoggedIn = true;
                    });
                        
                    string content = await GetHttpContentWithTokenAsync(authResult.AccessToken);
                    UpdateUserContent(content);
                    
                    Device.BeginInvokeOnMainThread(() => { pLoadingMessage = "Navigating to lobby"; });
                    App.Current.MainPage = App.vPagesHelper.pHomePage;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Authentication failed. See exception message for details: ", ex.Message, "Dismiss");
            }

            IsBusy = false;
        }
    }
}
