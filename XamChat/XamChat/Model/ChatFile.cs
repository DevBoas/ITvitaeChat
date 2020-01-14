using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITvitaeChat2.Model
{
    public class ChatFile : ObservableObject
    {
        // User
        private string user;
        public string User
        {
            get => user;
            set => SetProperty(ref user, value);
        }

        // File
        private object file;
        public object File
        {
            get => file;
            set => SetProperty(ref file, value);
        }
    }
}
