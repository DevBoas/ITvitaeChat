using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ITvitaeChat2.Core.EventHandlers;
using ITvitaeChat2.Shared.Core.EventHandlers;
using Microsoft.AspNetCore.Http;

namespace ITvitaeChat2.Core
{
    public class ChatService
    {
        public event EventHandler<MessageEventArgs> OnReceivedMessage;

        public event EventHandler<FileEventArgs> OnReceivedFile;

        public event EventHandler<MessageEventArgs> OnEnteredOrExited;
        public event EventHandler<MessageEventArgs> OnConnectionClosed;

        private HubConnection hubConnection;
        private Random random;

        private bool IsConnected { get; set; }
        private Dictionary<string, string> ActiveChannels { get; } = new Dictionary<string, string>();


        public void Init(string urlRoot, string portNumber , bool useHttps)
        {

            //-----ITvitae server url = "10.10.1.34"-----//
            //-----ITvitae server port = "4444"-----//

            string url = $"http{(useHttps ? "s" : String.Empty)}://{urlRoot}:{portNumber}/hubs/chat";
            //string url = $"http://{urlRoot}:{portNumber}/hubs/chat";

            // Setup the hubconnection with the newly created url
            hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();

            // Initialize random so we can use it to randomize colors
            random = new Random();

            // Setup the hubConnection events
            hubConnection.Closed += async (error) =>
            {
                OnConnectionClosed?.Invoke(this, new MessageEventArgs("Connection closed...", DateTime.Now, string.Empty));
                IsConnected = false;

                // Wait a little and try to reconnect
                await Task.Delay(random.Next(0, 5) * 1000);
                try
                {
                    await ConnectAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            };

            // Receive message
            hubConnection.On<string, DateTime, string>("ReceiveMessage", (user, dateTime, message) =>
            {
                OnReceivedMessage?.Invoke(this, new MessageEventArgs(user, dateTime, message));
            });

            // Receive file
            hubConnection.On<string, DateTime, string, string>("ReceiveFile", (user, dateTime, folderName, fileName) =>
            {
                OnReceivedFile?.Invoke(this, new FileEventArgs(user, dateTime, folderName, fileName));
            });

            // Entered 
            hubConnection.On<string, DateTime>("Entered", (user, dateTime) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs(user, dateTime, $"{user} entered."));
            });

            // Exited
            hubConnection.On<string, DateTime>("Left", (user, dateTime) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs(user, dateTime, $"{user} left."));
            });

            // Entered or left ??? <--- WTF (vanuit tutorial, geen idee waar voor nodig)
            hubConnection.On<string, DateTime>("EnteredOrLeft", (message, dateTime) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs("???Someone???", dateTime, message));                
            });
        }

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            await hubConnection.StartAsync();
            IsConnected = true;
        }

        public async Task DisconnectAsync()
        {
            if (!IsConnected)
                return;

            try
            {
                await hubConnection.DisposeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            ActiveChannels.Clear();
            IsConnected = false;
        }

        public async Task LeaveChannelAsync(string group, string userName, DateTime dateTime)
        {
            if (!IsConnected || !ActiveChannels.ContainsKey(group))
                return;
          
            await hubConnection.SendAsync("RemoveFromGroup", group, userName, dateTime);

            ActiveChannels.Remove(group);
        }

        public async Task JoinChannelAsync(string group, string userName, DateTime dateTime)
        {
            if (!IsConnected || ActiveChannels.ContainsKey(group))
                return;
        
            await hubConnection.SendAsync("AddToGroup", group, userName, dateTime);
            ActiveChannels.Add(group, userName);

        }

        public async Task SendMessageAsync(string group, string userName, DateTime dateTime, string message)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            await hubConnection.InvokeAsync("SendMessageGroup", group, userName, dateTime, message);
        }

        public async Task SendFileAsync(string group, string userName, DateTime dateTime, string folderName, string fileName)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            await hubConnection.InvokeAsync("SendFileGroup", group, userName, dateTime, folderName, fileName);
        }

        public List<string> GetRooms()
        {
            return new List<string>
                        {
                                "Xam",
                                "Xamarana",
                                "Xam Xam 4 life"
                        };
        }
    }
}
