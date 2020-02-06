using Xamarin.Forms;
using ITvitaeChat2.Model;
using System.Threading.Tasks;
using System;
using ITvitaeChat2.Helpers;
using System.Linq;
using System.Collections.ObjectModel;
using Plugin.FilePicker;
using ITvitaeChat2.Services;
using System.Net.Http;
using Plugin.FilePicker.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using PCLStorage;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.IO;
using System.Windows.Input;

namespace ITvitaeChat2.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        #region Variables
        // DialogService
        private DialogServices DialogServices;

        private Random random;

        private static readonly string[] ImageExtensions = new string[] { "jpeg", "jpg", "exif", "tiff", "gif", "bmp", "png" };
        private static readonly string svg = "svg";
        #endregion

        #region Properties
        // Chat file
        public ChatFile ChatFile { get; }

        // Chat message
        public ChatMessage ChatMessage { get; }

        // Collection containing abstract baseclass of messages. Can hold things like text messages but also file messages
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

        public ICommand SendFileCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand DownloadFileCommand { get; }
        public ICommand SelectionChangedCommand { get; }
        public ICommand pOKClickedCommand { get; }
        public ICommand pExitCommand { get; }
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
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
            pOKClickedCommand = new Command(() => OKClicked());
            pExitCommand = new Command(async () => await Exit());

            random = new Random();

            // Initiate the connection between client and server
            ChatService.Init(Settings.ServerIP, Settings.ServerPort, Settings.UseHttps);

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
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = true;
                    pIsRunning = true;
                    pLoadingMessageTitle = "Connecting";
                    pLoadingMessage = $"Please wait while we connect you to {Settings.Group}.";
                });

                await ChatService.ConnectAsync();
                await ChatService.JoinChannelAsync(Settings.Group, Settings.UserFirstName, DateTime.Now);
                IsConnected = true;

                AddRemoveUser(Settings.UserFirstName, true);
                await Task.Delay(500);

                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = true;
                    pIsRunning = false;
                    pHasOKButton = true;
                    pLoadingMessageTitle = "Connected";
                    pLoadingMessage = $"You succesfully connected to {Settings.Group}.\n\n Click 'OK' to continue.";
                });
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = true;
                    pIsRunning = false;
                    pHasOKButton = true;
                    pLoadingMessageTitle = "Connection failed";
                    pLoadingMessage = $"The following error message was received while connecting to {Settings.Group}:\n{ex.Message}\n\n Click 'OK' to continue.";
                });
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
        /// Let's user pick any file from the device and send it to every one in the chatroom via the server. This saves the file to the server for donwloading .
        /// </summary>
        /// <returns>void</returns>
        private async Task SendFile()
        {
            // Check if client is connected to the Server hub
            if (!IsConnected)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = true;
                    pHasOKButton = true;
                    pLoadingMessageTitle = "Not connected";
                    pLoadingMessage = "Please connect to the server and try again. You can do this at your profile page.\n\n Click 'OK' to continue.";
                });
                return;
            }

            // Let user pick a file
            FileData fileData = await CrossFilePicker.Current.PickFile();

            // Check if file is null
            if (fileData == null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = true;
                    pHasOKButton = true;
                    pLoadingMessageTitle = "File not found or selected";
                    pLoadingMessage = "Please select the file you want to send.\n\n Click 'OK' to continue.";
                });
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
                    // For each parameter in the API fill that parameter. In this case the folder name and the file itself.
                    using (MultipartFormDataContent formData = new MultipartFormDataContent())
                    {
                        formData.Add(new StringContent(Settings.UserEmail), "folderName");
                        formData.Add(fileStreamContent);
                        HttpResponseMessage responseMessage = await client.PostAsync(postUri, formData);

                        //await DialogServices.DisplayAlert("Response from server", $"Is succes: {responseMessage.IsSuccessStatusCode}\nMessage: {await responseMessage.Content.ReadAsStringAsync()}", "OK");

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
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = true;
                    pHasOKButton = true;
                    pLoadingMessageTitle = "Sending file failed";
                    pLoadingMessage = $"The following error message was received:\n{ex.Message}.\n\n Click 'OK' to continue.";
                });
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
        /// <param name="_user">The user that sends the file</param>
        private void SendLocalFile(string _user, DateTime dateTime, string folderName, string fileName)
        {
            User user = Users.FirstOrDefault(u => u.Name == _user);

            ChatFile chatFile = new ChatFile();
            chatFile.FolderName = folderName;
            chatFile.FileName = fileName;
            chatFile.MessageDate = dateTime;
            chatFile.User = _user;
            chatFile.Color = user?.Color ?? Color.FromRgba(0, 0, 0, 0);

            Device.BeginInvokeOnMainThread(() =>
            {
                if (IsImage(fileName))
                {
                    chatFile.Thumbnail = "outline_image_white_48.png"; //$"http{(Settings.UseHttps ? "s" : string.Empty)}://{Settings.ServerIP}:{Settings.ServerPort}/{folderName}/{fileName}";
                }

                BaseMessages.Insert(0, chatFile);
            });
        }

        /// <summary>
        /// Downloads a file to the local storage/pictures/ITvitaeDownloads
        /// </summary>
        /// <param name="chatFile"></param>
        /// <returns></returns>
        private async Task DownloadFile(ChatFile chatFile)
        {
            // Check if client is connected to the Server hub
            if (!IsConnected)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = true;
                    pHasOKButton = true;
                    pLoadingMessageTitle = "Not connected";
                    pLoadingMessage = "Please connect to the server and try again. You can do this at your profile page.\n\n Click 'OK' to continue.";
                });
                return;
            }

            // Check for runtime permission
            if (await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage) != PermissionStatus.Granted)
            {
                var response = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                if (response[Permission.Storage] != PermissionStatus.Granted)
                {
                    return;
                }
            }

            // Preparing variables and properties
            Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = true;
                pLoadingMessageTitle = "Dowloading file";
                pLoadingMessage = $"Busy downloading {chatFile.FileName}. Please wait for a moment.";
            });
            IFolder itvitaeFolder = null;
            IFile file = null;
            Uri getUri = new Uri($"http{(Settings.UseHttps ? "s" : String.Empty)}://{Settings.ServerIP}:{Settings.ServerPort}/" +
                    $"api/files?folderName={chatFile.FolderName}&fileName={chatFile.FileName}");

            // Try to get the file
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    HttpResponseMessage responseMessage = await client.GetAsync(getUri);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        // Create a new folder if it doesn't exist yet
                        itvitaeFolder = await SpecialFolder.Current.Pictures.CreateFolderAsync("ITvitaeDownloads", CreationCollisionOption.OpenIfExists);

                        // Create a file
                        file = await itvitaeFolder.CreateFileAsync(chatFile.FileName, CreationCollisionOption.GenerateUniqueName);

                        // Get the file data as byte array
                        byte[] fileData = await responseMessage.Content.ReadAsByteArrayAsync();

                        // Fill the newly created file with data
                        using (System.IO.Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            stream.Write(fileData, 0, fileData.Length);
                        }

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            pLoadingMessageTitle = "Download completed";
                            pLoadingMessage = $"Your downloaded file is located at:\n{file.Path}.\n\nClick 'OK' to continue";
                            pHasOKButton = true;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    pHasOKButton = true;
                    pLoadingMessageTitle = "Download failed";
                    pLoadingMessage = $"The following error occured:\n{ex.Message}\nClick 'OK' to continue.";
                });
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() => { pIsRunning = false; });
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

        /// <summary>
        /// Task for executing the donwload command if the message is a file. Acts as an OnClick event.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <returns></returns>
        private Task SelectionChanged(object selectedItem)
        {
            if (selectedItem is ChatFile)
            {
                DownloadFileCommand.Execute(selectedItem as ChatFile);
            }
            Device.BeginInvokeOnMainThread(() => { SelectedItem = null; });
            return Task.CompletedTask;
        }

        /// <summary>
        /// Method for checking if any file is an image. (Not all image extensions are supported.)
        /// </summary>
        /// <param name="filename">name of file and its extension</param>
        /// <returns>True if the file is an image</returns>
        public bool IsImage(string filename)
        {
            // Get the file extension now so we only have to initialize it once and not every loop
            string extension = Path.GetExtension(filename).ToLower();

            // Check if the file is allowed
            for (byte i = 0; i < ImageExtensions.Length; i++)
            {
                if (extension.Equals($".{ImageExtensions[i]}"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Popup is dismissedand is removed/ made invisible
        /// </summary>
        private void OKClicked()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                pIsRunning = false;
                IsBusy = false;
                pHasOKButton = false;
            });
        }

        /// <summary>
        /// Exits the current page/chat room
        /// </summary>
        /// <returns>Void</returns>
        private async Task Exit()
        {
            await App.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
