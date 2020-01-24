using System;

namespace ITvitaeChat2.Core.EventHandlers
{
    public class MessageEventArgs : IMessageEventArgs
    {
        public MessageEventArgs(string user, DateTime dateTime, string message)
        {
            Message = message;
            DateTime = dateTime;
            User = user;
        }

        public string Message { get; }
        public DateTime DateTime { get; }
        public string User { get; }
    }
}
