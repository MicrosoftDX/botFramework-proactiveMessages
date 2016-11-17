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

namespace startNewDialog
{
    public class ConversationStarter
    {
        //This will interrupt the conversation and send the user to SurveyDialog, then wait until that's done 
        public static async Task Resume(string resumptionCookie)
        {
            var message = ResumptionCookie.GZipDeserialize(resumptionCookie).GetMessage();
            var client = new ConnectorClient(new Uri(message.ServiceUrl));

            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
            {
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(CancellationToken.None);

                //This is our dialog stack
                var stack = scope.Resolve<IDialogStack>();
                
                //interrupt the stack. This means that we're stopping whatever conversation that is currently happening with the user
                //Then adding this stack to run and once it's finished, we will be back to the original conversation
                var dialog =new SurveyDialog();
                stack.Call(dialog.Void<object, IMessageActivity>(), null);
                await stack.PollAsync(CancellationToken.None);

                //flush dialog stack
                await botData.FlushAsync(CancellationToken.None);
           
            }
        }
    }
}