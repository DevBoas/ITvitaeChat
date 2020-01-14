﻿using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Platforms.iOS;

namespace ITvitaeChat2.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            ImageCircle.Forms.Plugin.iOS.ImageCircleRenderer.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();
            LoadApplication(new App());

            App.ParentWindow = new UIViewController();

            UITabBar.Appearance.SelectedImageTintColor = ((Color)App.Current.Resources["SecondaryColor"]).ToUIColor();

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, string sourceApplication, NSObject annotation)
        {
            if (AuthenticationContinuationHelper.IsBrokerResponse(sourceApplication))
            {
                AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url);
                return true;
            }

            else if (!AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url))
            {
                return false;
            }

            return true;
        }
    }
}