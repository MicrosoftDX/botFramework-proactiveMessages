using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace simpleSendMessage
{
    public class ConversationStarter
    {
        //Note: Of course you don't want these here. Eventually you will need to save these in some table
        //Having them here as static variables means we can only remember one user :)
        public static string fromId;
        public static string fromName;
        public static string toId;
        public static string toName;
        public static string serviceUrl;

        //This will send an adhoc message to the user
        public static async Task Resume()
        {
            var userAccount = new ChannelAccount(toId,toName);
            var botAccount = new ChannelAccount(fromId, fromName);
            var connector = new ConnectorClient(new Uri(serviceUrl));
            var conversationId = await connector.Conversations.CreateDirectConversationAsync( botAccount, userAccount);

            IMessageActivity message = Activity.CreateMessageActivity();
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId.Id);
            message.Text = "Hello, this is a notification";
            message.Locale = "en-Us";
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }
    }
}