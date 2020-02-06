using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using FFImageLoading.Forms;
using Xamarin.Forms;
using System.Threading.Tasks;
using ITvitaeChat2.Services;
using Microsoft.AspNetCore.Http;

namespace ITvitaeChat2.Model
{
    public class ChatFile : BaseMessage
    {

        private string fileName;
        public string FileName
        {
            get => fileName;
            set => SetProperty(ref fileName, value);
        }

        private string folderName;
        public string FolderName
        {
            get => folderName;
            set => SetProperty(ref folderName, value);
        }

        // Thumbnail (If file is an image, show the image. Else show generic thumbnail)
        private string thumbnail;
        public string Thumbnail
        {
            get => thumbnail;
            set => SetProperty(ref thumbnail, value);
        }
    }
}

