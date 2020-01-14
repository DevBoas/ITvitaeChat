
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using ITvitaeChat2.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ITvitaeChat2.View;

[assembly: ExportRenderer(typeof(LobbyPage), typeof(MainPageRenderer))]
namespace ITvitaeChat2.iOS
{
    class MainPageRenderer : PageRenderer
    {
        LobbyPage page;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as LobbyPage;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
    }
}