using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace ITvitaeChat2.Model
{
    public class ChatMessage : BaseMessage
    {
        // Message
        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }
    }
}
