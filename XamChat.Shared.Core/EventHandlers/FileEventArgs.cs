using ITvitaeChat2.Core.EventHandlers;
using ITvitaeChat2.Shared.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITvitaeChat2.Shared.Core.EventHandlers
{
    public class FileEventArgs : IFileEventArgs
    {
        public FileEventArgs(string user, DateTime dateTime, IFormFile file)
        {
            User = user;
            DateTime = dateTime;
            File = file;
        }

        public string User { get; }
        public DateTime DateTime { get; }
        public IFormFile File { get; }
    }

    
}
