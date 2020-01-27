using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITvitaeChat2.Backend.Hubs
{
    /// <summary>
    /// These are Tasks that can be invoked by the clientside Chatservice class inside the shared core project. 
    /// </summary>

    public class ChatHub : Hub
    {
        public async Task AddToGroup(string groupName, string user, DateTime dateTime)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Entered", user, dateTime);
        }

        public async Task RemoveFromGroup(string groupName, string user, DateTime dateTime)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Left", user, dateTime);
        }

        public async Task SendMessageGroup(string groupName, string user, DateTime dateTime, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, dateTime, message);
        }

        public async Task SendFileGroup(string groupName, string user, DateTime dateTime, string folderName, string fileName)
        {
            await Clients.Group(groupName).SendAsync("ReceiveFile", user, dateTime, folderName, fileName);
        }
    }
}
