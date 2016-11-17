using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace simpleSendMessage
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        [NonSerialized]
        Timer t;
       
        string fromId;
        string fromName;

        string toId;
        string toName;
        string serviceUrl;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            //We need to keep this data so we know who to send the message to. Assume this would be stored somewhere, e.g. an Azure Table
            this.toId = message.From.Id;
            this.toName = message.From.Name;
            this.fromId = message.Recipient.Id;
            this.fromName = message.Recipient.Name;
            this.serviceUrl = message.ServiceUrl;

            //We create a timer to simulate some background process or trigger
            t = new Timer(new TimerCallback(timerEvent));
            t.Change(5000, Timeout.Infinite);

            //We now tell the user that we will talk to them in a few seconds
            await context.PostAsync("Hello! In a few seconds I'll send you a message proactively to demonstrate how bots can initiate messages");
            context.Wait(MessageReceivedAsync);
        }
        public void timerEvent(object target)
        {
            
            t.Dispose();
            ConversationStarter.Resume(this.fromId,this.fromName,this.toId,this.toName,this.serviceUrl); //We don't need to wait for this, just want to start the interruption here
        }


    }
}