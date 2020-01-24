using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using FFImageLoading.Forms;
using Xamarin.Forms;
using System.Threading.Tasks;
using ITvitaeChat2.Services;

namespace ITvitaeChat2.Model
{
    public class ChatFile : BaseMessage
    {
        //public ChatFile()
        //{
        //    DialogServices = new DialogServices();
        //    DownloadFileCommand = new Command(async () => await DownloadFile());
        //}

        // Dialog service
        private DialogServices DialogServices;

        // File (Can be anything)
        private object file;
        public object File
        {
            get => file;
            set => SetProperty(ref file, value);
        }

        // Thumbnail (If file is an image, show the image. Else show generic thumbnail)
        private string thumbnail;
        public string Thumbnail
        {
            get => thumbnail;
            set => SetProperty(ref thumbnail, value);
        }

        //public Command DownloadFileCommand;

        //public async Task DownloadFile()
        //{
        //    await DialogServices.DisplayAlert("Download file...", "Chatfile clicked!", "OK");
        //}
    }
}

