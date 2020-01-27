using System;
using Xamarin.Essentials;

namespace ITvitaeChat2.Helpers
{
    public static class Settings
    {
        public static string AppCenterAndroid = "AC_ANDROID";

        #if DEBUG
            private static readonly string defaultIP = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
            private static readonly string defaultPort = DeviceInfo.Platform == DevicePlatform.Android ? "5000" : "5001";
        #else
            private static readonly string defaultIP = "10.10.1.34";
            private static readonly string defaultPort = "4444";
        #endif

        public static bool UseHttps
        {
            get => (ServerIP != "localhost" && ServerIP != "10.0.2.2" && ServerIP != String.Empty);
        }

        public static string ServerIP
        {
            get 
            {
                if (Preferences.Get(nameof(ServerIP), defaultIP).Equals(String.Empty))
                {
                    return defaultIP;
                }
                else
                {
                    return Preferences.Get(nameof(ServerIP), defaultIP);
                }
                    
            }
            set => Preferences.Set(nameof(ServerIP), value);
        }

        public static string ServerPort
        {
            get 
            {
                if (Preferences.Get(nameof(ServerPort), defaultPort).Equals(String.Empty))
                {
                    return defaultPort;
                }
                else
                {
                    return Preferences.Get(nameof(ServerPort), defaultPort);
                }
            } 
            set => Preferences.Set(nameof(ServerPort), value);
        }

        private static readonly string defaultFirstName = $"{DeviceInfo.Platform} User firstname";
        public static string UserFirstName
        {
            get => Preferences.Get(nameof(UserFirstName), defaultFirstName);
            set => Preferences.Set(nameof(UserFirstName), value);
        }

        private static readonly string defaultLastName = "User lastname";
        public static string UserLastName
        {
            get => Preferences.Get(nameof(UserLastName), defaultLastName);
            set => Preferences.Set(nameof(UserLastName), value);
        }

        //TODO Store user image
        private static readonly string defaultPicture = "tab_person.png";
        public static string UserPicture
        {
            get => Preferences.Get(nameof(UserPicture), defaultPicture);
            set => Preferences.Set(nameof(UserPicture), value);
        }

        private static readonly string defaultEmail = "firstname.lastname@itvitaelearning.nl";
        public static string UserEmail
        {
            get => Preferences.Get(nameof(UserEmail), defaultEmail);
            set => Preferences.Set(nameof(UserEmail), value);
        }
        
        public static string Group
        {
            get => Preferences.Get(nameof(Group), string.Empty);
            set => Preferences.Set(nameof(Group), value);
        }
    }
}
