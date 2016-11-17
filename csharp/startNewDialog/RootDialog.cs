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

namespace startNewDialog
{


    [Serializable]
    public class RootDialog : IDialog<object>
    {
        [NonSerialized]
        Timer t;
        
        string resumptionCookie;
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            resumptionCookie = new ResumptionCookie(message).GZipSerialize();

            //We will start a timer to fake a background service that will trigger the proactive message

            t = new Timer(new TimerCallback(timerEvent));
            t.Change(5000, Timeout.Infinite);

            //We echo the message regardless
            await context.PostAsync("Hey there, I'm going to interrupt our conversation and start a survey in a few seconds");
            context.Wait(MessageReceivedAsync);
        }
        public void timerEvent(object target)
        {
            
            t.Dispose();
            ConversationStarter.Resume(resumptionCookie); //We don't need to wait for this, just want to start the interruption here
        }


    }
}