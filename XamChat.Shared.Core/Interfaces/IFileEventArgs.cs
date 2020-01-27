using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITvitaeChat2.Shared.Core.Interfaces
{
    interface IFileEventArgs
    {
        IFormFile File { get; }
    }
}
