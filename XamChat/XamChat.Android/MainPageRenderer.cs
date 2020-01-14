
using Android.App;
using Android.Content;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using ITvitaeChat2.Droid;
using ITvitaeChat2.View;

[assembly: ExportRenderer(typeof(LobbyPage), typeof(MainPageRenderer))]
namespace ITvitaeChat2.Droid
{
    class MainPageRenderer : PageRenderer
    {
        public MainPageRenderer(Context context) : base(context)
        {

        }
        LobbyPage page;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as LobbyPage;
            var activity = this.Context as Activity;
        }
    }
}