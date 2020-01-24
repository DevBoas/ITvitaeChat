using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ITvitaeChat2.Model
{
    public abstract class BaseMessage : ObservableObject
    {
        public Random random = new Random();

        // User
        private string user;
        public string User
        {
            get => user;
            set => SetProperty(ref user, value);
        }

        // Date of sending
        private DateTime messageDate = DateTime.Now;
        public DateTime MessageDate
        {
            get => messageDate;
            set => SetProperty(ref messageDate, value);
        }

        // Tick
        private string[] tick = new string[] { "✔", "✔" };
        public string[] Tick
        {
            get => tick;
            set => SetProperty(ref tick, value);
        }

        // 
        public string NewFormatTest
        {
            get
            {
                return string.Format("{0:hh:mm} {1}", MessageDate, Tick);
            }
        }

        // First letter of name
        private string firstLetter;
        public string FirstLetter
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(firstLetter))
                    return firstLetter;

                firstLetter = User?.Length > 0 ? User[0].ToString() : "?";
                return firstLetter;
            }
            set => firstLetter = value;
        }

        // Color for frame with the first letter
        Color color;
        public Color Color
        {
            get
            {
                if (color != null && color.A != 0)
                    return color;

                color = Color.FromRgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)).MultiplyAlpha(.9);
                return color;
            }
            set => color = value;
        }

        // Color for frame with the message
        private Color backgroundColor;
        public Color BackgroundColor
        {
            get
            {
                if (backgroundColor != null && backgroundColor.A != 0)
                    return backgroundColor;

                backgroundColor = Color.MultiplyAlpha(.6);
                return backgroundColor;
            }
            set => backgroundColor = value;
        }
    }
}
