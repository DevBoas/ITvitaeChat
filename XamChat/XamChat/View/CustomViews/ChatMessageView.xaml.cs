using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ITvitaeChat2.View.CustomViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatMessageView : ContentView
    {
        public ChatMessageView()
        {
            InitializeComponent();
        }
    }
}