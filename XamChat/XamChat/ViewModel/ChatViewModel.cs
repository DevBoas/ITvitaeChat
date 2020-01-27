using MvvmHelpers;
using Xamarin.Forms;
using ITvitaeChat2.Model;
using System.Threading.Tasks;
using System;
using ITvitaeChat2.Helpers;
using System.Linq;
using System.Collections.ObjectModel;
using Plugin.FilePicker;
using System.Diagnostics;
using ITvitaeChat2.Services;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Plugin.FilePicker.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;

namespace ITvitaeChat2.ViewModel
{
    /// <summary>
    /// Class containing Microsoft.AspNetCore.Http.IFormFile
    /// </summary>
    //public class FileUploadAPI
    //{
    //    public IFormFile files { get; set; }
    //}

    public class ChatViewModel : ViewModelBase
    {
        // DialogService
        private DialogServices DialogServices;

        // Chat file
        public ChatFile ChatFile { get; }

        // Chat message
        public ChatMessage ChatMessage { get; }

        // Collection containing abstract baseclass of messages. Can hold things like string messages but also file messages
        public ObservableCollection<BaseMessage> BaseMessages { get; }

        // Selected item
        private BaseMessage selectedItem;
        public BaseMessage SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        // Users
        public ObservableCollection<User> Users { get; }

        private bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetProperty(ref isConnected, value);
                });
            }
        }
        
        public Command SendFileCommand { get; }
        public Command SendMessageCommand { get; }
        public Command ConnectCommand { get; }
        public Command DisconnectCommand { get; }
        public Command DownloadFileCommand { get; }
        public Command SelectionChangedCommand { get; }

        Random random;
        public ChatViewModel()
        {
            if (DesignMode.IsDesignModeEnabled)
                return;

            Title = Settings.Group;

            DialogServices = new DialogServices();

            ChatMessage = new ChatMessage();
            ChatFile = new ChatFile();
            BaseMessages = new ObservableCollection<BaseMessage>();
            Users = new ObservableCollection<User>();

            SendMessageCommand = new Command(async () => await SendMessage());
            ConnectCommand = new Command(async () => await Connect());
            DisconnectCommand = new Command(async () => await Disconnect());
            SendFileCommand = new Command(async () => await SendFile());
            DownloadFileCommand = new Command(async (object chatFile) => await DownloadFile(chatFile as ChatFile));
            SelectionChangedCommand = new Command(async (object selectedItem) => await SelectionChanged(selectedItem));

            random = new Random();

            // Initiate the connection between client and server
            ChatService.Init(Settings.ServerIP, Settings.ServerPort , Settings.UseHttps);

            // Received message event handling
            ChatService.OnReceivedMessage += (sender, args) =>
            {
                SendLocalMessage(args.User, args.DateTime, args.Message);
                AddRemoveUser(args.User, true);
            };

            // Received file event handling
            ChatService.OnReceivedFile += (sender, args) =>
            {
                SendLocalFile(args.User, args.DateTime, args.FolderName, args.FileName);
                AddRemoveUser(args.User, true);
            };

            // Leave or enter event handling
            ChatService.OnEnteredOrExited += (sender, args) =>
            {
                AddRemoveUser(args.User, args.Message.Contains("entered"));
            };

            // Connection closed event handling
            ChatService.OnConnectionClosed += (sender, args) =>
            {
                SendLocalMessage(args.User, args.DateTime, args.Message);  
            };
        }

        /// <summary>
        /// Method for connecting to the server
        /// </summary>
        /// <returns></returns>
        private async Task Connect()
        {
            if (IsConnected)
                return;
            try
            {
                IsBusy = true;
                await ChatService.ConnectAsync();
                await ChatService.JoinChannelAsync(Settings.Group, Settings.UserFirstName, DateTime.Now);
                IsConnected = true;

                AddRemoveUser(Settings.UserFirstName, true);
                await Task.Delay(500);
                SendLocalMessage(Settings.UserFirstName, DateTime.Now, "Connected...");
            }
            catch (Exception ex)
            {
                SendLocalMessage(Settings.UserFirstName, DateTime.Now, $"Connection error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Method for disconnecting from the server signalR hub
        /// </summary>
        /// <returns>void</returns>
        private async Task Disconnect()
        {
            if (!IsConnected)
                return;
            await ChatService.LeaveChannelAsync(Settings.Group, Settings.UserFirstName, DateTime.Now);
            await ChatService.DisconnectAsync();
            IsConnected = false;
            SendLocalMessage(Settings.UserFirstName, DateTime.Now, "Disconnected...");
        }

        /// <summary>
        /// Let's user select any file from his device and send it to every one in the chatroom
        /// </summary>
        /// <returns>void</returns>
        private async Task SendFile()
        {
            // Check if client is connected to the Server hub
            if (!IsConnected)
            {
                await DialogService.DisplayAlert("Not connected", "Please connect to the server and try again.", "OK");
                return;
            }

            // Let user pick a file
            FileData fileData = await CrossFilePicker.Current.PickFile();

            // Check if file is null
            if (fileData == null)
            {
                await DialogServices.DisplayAlert("File not found or selected", "Please select the file you want to send", "OK");
                return;
            }

            IFormFile formFile = new StreamContent(fileData.GetStream()) as IFormFile;

            // Try to send the selected file
            try
            {
                IsBusy = true;

                // The address to post to
                Uri postUri = new Uri($"http{(Settings.UseHttps ? "s" : String.Empty)}://{Settings.ServerIP}:{Settings.ServerPort}/api/files");

                // Convert user picked file to HttpContent containing StreamContent. Streamcontent is good in case of larger files.
                HttpContent fileStreamContent = new StreamContent(fileData.GetStream());
                fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "formFile", FileName = fileData.FileName };
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                using (HttpClient client = new HttpClient())
                {
                    // For each parameter in the API add that parameter. In this case the folder name and the file itself.
                    using (MultipartFormDataContent formData = new MultipartFormDataContent())
                    {
                        formData.Add(new StringContent(Settings.UserEmail), "folderName");
                        formData.Add(fileStreamContent);
                        HttpResponseMessage responseMessage = await client.PostAsync(postUri, formData);

                        await DialogServices.DisplayAlert("Response from server", $"Is succes: {responseMessage.IsSuccessStatusCode}\nMessage: {await responseMessage.Content.ReadAsStringAsync()}", "OK");

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            //SendLocalFile(Settings.UserFirstName, DateTime.Now, formFile);
                            await ChatService.SendFileAsync(Settings.Group, Settings.UserFirstName, DateTime.Now, Settings.UserEmail, fileData.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendLocalMessage(Settings.UserFirstName, DateTime.Now, $"Send failed: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
            
        }

        /// <summary>
        /// Add this file only locally to the base message collection
        /// </summary>
        /// <param name="file">The file</param>
        /// <param name="dateTime">The datetime of send</param>
        /// <param name="user">The user that sends the file</param>
        private void SendLocalFile(string user, DateTime dateTime, string folderName, string fileName)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var first = Users.FirstOrDefault(u => u.Name == user);

                BaseMessages.Insert(0, new ChatFile
                {
                    FolderName = folderName,
                    FileName = fileName,
                    MessageDate = dateTime,
                    User = user,
                    Color = first?.Color ?? Color.FromRgba(0, 0, 0, 0)
                });
            });
        }

        private async Task DownloadFile(ChatFile chatFile)
        {
            // Check if client is connected to the Server hub
            if (!IsConnected)
            {
                await DialogService.DisplayAlert("Not connected", "Please connect to the server and try again.", "OK");
                return;
            }

            // Try to get the file
            try
            {
                IsBusy = true;

                // The address get from
                Uri getUri = new Uri($"http{(Settings.UseHttps ? "s" : String.Empty)}://{Settings.ServerIP}:{Settings.ServerPort}/" +
                    $"api/files?folderName={chatFile.FolderName}&fileName={chatFile.FileName}");

                using (HttpClient client = new HttpClient())
                {
                    // For each parameter in the API add that parameter. In this case the folder name and the file name.
                    //using (MultipartFormDataContent getData = new MultipartFormDataContent())
                    //{
                    //    getData.Add(new StringContent(chatFile.FolderName), "folderName");
                    //    getData.Add(new StringContent(chatFile.FileName), "fileName");

                    //}

                    HttpResponseMessage responseMessage = await client.GetAsync(getUri);

                    //await DialogServices.DisplayAlert("Response from server", $"Is succes: {responseMessage.IsSuccessStatusCode}\nMessage: {await responseMessage.Content.ReadAsStringAsync()}", "OK");

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        //string contentStream = await responseMessage.Content.ReadAsStringAsync();
                        string fullFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), chatFile.FileName);
                        byte[] fileByteArray = await responseMessage.Content.ReadAsByteArrayAsync();
                        File.WriteAllBytes(fullFilePath, fileByteArray);
                    }
                }
            }
            catch (Exception ex)
            {
                SendLocalMessage(Settings.UserFirstName, DateTime.Now, $"Send failed: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Send a message to everyone in the chatroom
        /// </summary>
        /// <returns>Void</returns>
        private async Task SendMessage()
        {
            // Check if client is connected to the Server hub
            if (!IsConnected)
            {
                await DialogService.DisplayAlert("Not connected", "Please connect to the server and try again.", "OK");
                return;
            }

            // Try to send the message
            try
            {
                IsBusy = true;
                await ChatService.SendMessageAsync(Settings.Group, Settings.UserFirstName, DateTime.Now, ChatMessage.Message);

                ChatMessage.Message = string.Empty;
            }
            catch (Exception ex)
            {
                SendLocalMessage($"Send failed: {ex.Message}", DateTime.Now, Settings.UserFirstName);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Add this message only locally to the base message collection
        /// </summary>
        /// <param name="message"></param>
        /// <param name="dateTime"></param>
        /// <param name="user"></param>
        private void SendLocalMessage(string user, DateTime dateTime, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var first = Users.FirstOrDefault(u => u.Name == user);

                BaseMessages.Insert(0, new ChatMessage
                {
                    Message = message,
                    MessageDate = dateTime,
                    User = user,
                    Color = first?.Color ?? Color.FromRgba(0, 0, 0, 0)
                });
            });
        }

        /// <summary>
        /// Method for adding or removing a User.
        /// </summary>
        /// <param name="name">User to be added or removed</param>
        /// <param name="add">True if adding, false if removing</param>
        private void AddRemoveUser(string name, bool add)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            if (add)
            {
                if (!Users.Any(u => u.Name == name))
                {
                    var color = BaseMessages.FirstOrDefault(m => m.User == name)?.Color ?? Color.FromRgba(0, 0, 0, 0);
                    Device.BeginInvokeOnMainThread(() =>
                        {
                            Users.Add(new User { Name = name, Color = color });
                        });
                }
            }
            else
            {
                var user = Users.FirstOrDefault(u => u.Name == name);
                if (user != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Users.Remove(user);
                    });
                }
            }
        }

        private Task SelectionChanged(object selectedItem)
        {
            if (selectedItem is ChatFile)
            {
                DownloadFileCommand.Execute(selectedItem as ChatFile);
            }

            SelectedItem = null;
            return Task.CompletedTask;
        }
    }


}
