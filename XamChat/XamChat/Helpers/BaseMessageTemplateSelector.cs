using ITvitaeChat2.Model;
using ITvitaeChat2.View.CustomViews;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ITvitaeChat2.Helpers
{
    /// <summary>
    /// Class used for seeing what type of Chatmessage has been sent so we can use the apropiate view/template for it.
    /// </summary>
    public class BaseMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ChatMessageTemplate { get; set; }
        public DataTemplate ChatFileTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object model, BindableObject container)
        {
            if (model is ChatMessage)
            {
                return ChatMessageTemplate;
            }
            else if (model is ChatFile)
            {
                return ChatFileTemplate;
            }

            // Default
            return ChatMessageTemplate;
        }
    }
}
