﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ITvitaeChat2.Core;
using ITvitaeChat2.Core.EventHandlers;

namespace ITvitaeChat2.ConsoleApp
{
    public class Program
    {
        static ChatService service;
        static string room;
        static string name;
        static Random random = new Random();
        public static async Task Main(string[] args)
        {
            name = "console " + random.Next(0, 10000);
            service = new ChatService();
            service.OnReceivedMessage += Service_OnReceivedMessage;
            Console.WriteLine("Enter IP:");
            var ip = Console.ReadLine();
            Console.WriteLine("Enter port number:");
            var port = Console.ReadLine();
            service.Init(ip, port, ip != "localhost");

            await service.ConnectAsync();
            Console.WriteLine("You are connected...");

            await JoinRoom();

            var keepGoing = true;
            do
            {
                var text = Console.ReadLine();
                if (text == "exit")
                {
                    keepGoing = false;
                }
                else if(text == "leave")
                {
                    await service.LeaveChannelAsync(room, name);
                    await JoinRoom();
                }
                else
                {
                    await service.SendMessageAsync(room, name, DateTime.Now, text);
                }
            }
            while (keepGoing);
        }

        static async Task JoinRoom()
        {
            Console.WriteLine($"Enter room ({string.Join(",", service.GetRooms())}):");
            room = Console.ReadLine();

            await service.JoinChannelAsync(room, name);
        }



        private static void Service_OnReceivedMessage(object sender, MessageEventArgs e)
        {
            if (e.User == name)
                return;
            Console.WriteLine(e.Message);
        }
    }
}
