using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ITvitaeChat2.Core.EventHandlers;
using ITvitaeChat2.Shared.Core.EventHandlers;

namespace ITvitaeChat2.Core
{
    public class ChatService
    {
        public event EventHandler<MessageEventArgs> OnReceivedMessage;

        public event EventHandler<MessageEventArgs> OnEnteredOrExited;
        public event EventHandler<MessageEventArgs> OnConnectionClosed;

        public event EventHandler<FileEventArgs> OnReceivedFile;

        HubConnection hubConnection;
        Random random;

        bool IsConnected { get; set; }
        Dictionary<string, string> ActiveChannels { get; } = new Dictionary<string, string>();


        public void Init(string urlRoot, bool useHttps)
        {
            random = new Random();

            var port = ":4444";

            //var port = (urlRoot == "localhost" || urlRoot == "10.0.2.2") ?
            //    (useHttps ? ":5001" : ":5000") :
            //    string.Empty;

            var url = $"http{(useHttps ? "s" : string.Empty)}://{urlRoot}{port}/hubs/chat";
            hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();


           hubConnection.Closed += async (error) =>
            {
                OnConnectionClosed?.Invoke(this, new MessageEventArgs("Connection closed...", string.Empty));
                IsConnected = false;
                await Task.Delay(random.Next(0, 5) * 1000);
                try
                {
                    await ConnectAsync();
                }
                catch (Exception ex)
                {
                    // Exception!
                    Debug.WriteLine(ex);
                }
            };

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                OnReceivedMessage?.Invoke(this, new MessageEventArgs(message, user));
            });

            hubConnection.On<string, object>("ReceiveFile", (user, file) =>
            {
                OnReceivedFile?.Invoke(this, new FileEventArgs(user, file));
            });

            hubConnection.On<string>("Entered", (user) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs($"{user} entered.", user));
            });


            hubConnection.On<string>("Left", (user) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs($"{user} left.", user));
            });


            hubConnection.On<string>("EnteredOrLeft", (message) =>
            {
                OnEnteredOrExited?.Invoke(this, new MessageEventArgs(message, message));                
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

        public async Task LeaveChannelAsync(string group, string userName)
        {
            if (!IsConnected || !ActiveChannels.ContainsKey(group))
                return;
          
            await hubConnection.SendAsync("RemoveFromGroup", group, userName);

            ActiveChannels.Remove(group);
        }

        public async Task JoinChannelAsync(string group, string userName)
        {
            if (!IsConnected || ActiveChannels.ContainsKey(group))
                return;
        
            await hubConnection.SendAsync("AddToGroup", group, userName);
            ActiveChannels.Add(group, userName);

        }

        public async Task SendMessageAsync(string group, string userName, string message)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            await hubConnection.InvokeAsync("SendMessageGroup", group, userName, message);
        }

        public async Task SendFileAsync(string group, string userName, object file)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            await hubConnection.InvokeAsync("ReceiveFile", group, userName, file);
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
