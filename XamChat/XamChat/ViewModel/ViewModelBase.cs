using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using ITvitaeChat2.Core;
using ITvitaeChat2.Interfaces;

namespace ITvitaeChat2.ViewModel
{
    public class ViewModelBase : BaseViewModel
    {
        ChatService chatService;
        public ChatService ChatService =>
            chatService ?? (chatService = DependencyService.Resolve<ChatService>());

        IDialogService dialogService;
        public IDialogService DialogService =>
            dialogService ?? (dialogService = DependencyService.Resolve<IDialogService>());

        private string loadingMessageTitle = "Loading, please wait...";
        public string pLoadingMessageTitle 
        {
            get => loadingMessageTitle;
            set => SetProperty(ref loadingMessageTitle, value);
        }

        private string loadingMessage = "Processing data";
        public string pLoadingMessage
        {
            get => loadingMessage;
            set => SetProperty(ref loadingMessage, value);
        }

        private bool IsRunning = false;
        public bool pIsRunning
        {
            get => IsRunning;
            set => SetProperty(ref IsRunning, value);
        }

        private bool HasOKButton = false;
        public bool pHasOKButton
        {
            get => HasOKButton;
            set => SetProperty(ref HasOKButton, value);
        }
    }
}
