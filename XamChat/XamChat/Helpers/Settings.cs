using System;
using Xamarin.Essentials;

namespace ITvitaeChat2.Helpers
{
    public static class Settings
    {
        public static string AppCenterAndroid = "AC_ANDROID";

#if DEBUG
        static readonly string defaultIP = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
#else
        static readonly string defaultIP = "xamchatr.azurewebsites.net";
#endif
        public static bool UseHttps
        {
            get => (defaultIP != "localhost" && defaultIP != "10.0.2.2");
        }

        public static string ServerIP
        {
            get => Preferences.Get(nameof(ServerIP), defaultIP);
            set => Preferences.Set(nameof(ServerIP), value);
        }

        static readonly string defaultFirstName = $"{DeviceInfo.Platform} User firstname";
        public static string UserFirstName
        {
            get => Preferences.Get(nameof(UserFirstName), defaultFirstName);
            set => Preferences.Set(nameof(UserFirstName), value);
        }

        static readonly string defaultLastName = "User lastname";
        public static string UserLastName
        {
            get => Preferences.Get(nameof(UserLastName), defaultLastName);
            set => Preferences.Set(nameof(UserLastName), value);
        }

        //TODO Store string containing the url of user image
        static readonly string defaultPicture = "tab_person.png";
        public static string UserPicture
        {
            get => Preferences.Get(nameof(UserPicture), defaultPicture);
            set => Preferences.Set(nameof(UserPicture), value);
        }

        static readonly string defaultEmail = "firstname.lastname@itvitaelearning.nl";
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
