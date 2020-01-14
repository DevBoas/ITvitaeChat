using ITvitaeChat2.Core.EventHandlers;
using ITvitaeChat2.Shared.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITvitaeChat2.Shared.Core.EventHandlers
{
    public class FileEventArgs : IFileEventArgs
    {
        public FileEventArgs(string user, object file)
        {
            User = user;
            File = file;
        }

        public string User { get; }
        public object File { get; }
    }

    
}
