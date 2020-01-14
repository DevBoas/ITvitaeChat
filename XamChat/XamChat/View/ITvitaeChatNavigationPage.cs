using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ITvitaeChat2.View
{
    public class ITvitaeChatNavigationPage : NavigationPage
    {
        public ITvitaeChatNavigationPage(Page page) : base(page)
        {

        }

        public ITvitaeChatNavigationPage() : base()
        {

        }

        void SetColor()
        {
            BarBackgroundColor = (Color)Application.Current.Resources["PrimaryColor"];
            BarTextColor = (Color)Application.Current.Resources["PrimaryTextColor"];
        }
    }
}
