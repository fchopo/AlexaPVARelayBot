using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using MyPVABot.Helpers.DirectLine;
using MyPVABot.Models.DirectLine;

using ActivityTypes = MyPVABot.Helpers.ActivityTypes;
using DirectLineClient = MyPVABot.Helpers.DirectLine.DirectLineClient;
using Microsoft.Extensions.Options;
using System.Runtime;


namespace MyPVABot.Bots
{
    public class myBotConnector: ActivityHandler
    {
        static DirectLineOptions options;
        static DirectLineClient directLineClient;        

        public myBotConnector(IOptions<MyBotConnectorSettings> myBotConnectorSettings) {
            var tokenEndpoint = myBotConnectorSettings.Value.TokenEndpoint;
            var regionalEndpoint = RegionalEndpointHelper.GetEndpointAsync(new Uri(tokenEndpoint)).Result;
            options = new DirectLineOptions(tokenEndpoint, regionalEndpoint);
            directLineClient = new DirectLineClient(options);
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<Microsoft.Bot.Schema.IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            
            await directLineClient.SendActivityAsync(new Microsoft.Bot.Connector.DirectLine.Activity()
            {
                Type = ActivityTypes.Message,
                From = new Microsoft.Bot.Connector.DirectLine.ChannelAccount { Id = turnContext.Activity.ChannelId },
                Text = turnContext.Activity.Text,
                Conversation = new Microsoft.Bot.Connector.DirectLine.ConversationAccount() { Id = turnContext.Activity.Conversation.Id }
            }, cancellationToken);

            // Get bot response using directlinClient
            List<Microsoft.Bot.Connector.DirectLine.Activity> responses = await GetBotResponseActivitiesAsync(directLineClient);

            string result = BotReplyAsAPIResponse(responses);

            await turnContext.SendActivityAsync(MessageFactory.Text(result, result), cancellationToken);
            
        }

        protected override async Task OnMembersAddedAsync(IList<Microsoft.Bot.Schema.ChannelAccount> membersAdded, ITurnContext<Microsoft.Bot.Schema.IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var tokenEndpoint = "https://c0e582a94e8fec09b6ee4584a9d7a2.5d.environment.api.powerplatform.com/powervirtualagents/botsbyschema/cr5fe_bizzSummitBot/directline/token?api-version=2022-03-01-preview";
            var regionalEndpoint = await RegionalEndpointHelper.GetEndpointAsync(new Uri(tokenEndpoint));
            var options = new DirectLineOptions(tokenEndpoint, regionalEndpoint);

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {                   
                    await directLineClient.SendActivityAsync(new Microsoft.Bot.Connector.DirectLine.Activity()
                    {
                        Type = ActivityTypes.Message,
                        From = new Microsoft.Bot.Connector.DirectLine.ChannelAccount { Id = Guid.NewGuid().ToString() },
                        Text = "Hello!",
                        Conversation = new Microsoft.Bot.Connector.DirectLine.ConversationAccount() { Id = Guid.NewGuid().ToString() }
                    }, cancellationToken);

                    // Get bot response using directlinClient
                    List<Microsoft.Bot.Connector.DirectLine.Activity> responses = await GetBotResponseActivitiesAsync(directLineClient);

                    string result = BotReplyAsAPIResponse(responses);
                    await turnContext.SendActivityAsync(MessageFactory.Text(result, result), cancellationToken);
                }
            }
        }

        private static async Task<List<Microsoft.Bot.Connector.DirectLine.Activity>> GetBotResponseActivitiesAsync(DirectLineClient directLineClient)
        {
            List<Microsoft.Bot.Connector.DirectLine.Activity> response = null;
            List<Microsoft.Bot.Connector.DirectLine.Activity> result = new List<Microsoft.Bot.Connector.DirectLine.Activity>();
            CancellationToken cts = new CancellationToken();
            do
            {
                response = await directLineClient.ReceiveActivitiesAsync(cts);
                if (response == null)
                {
                    directLineClient.Dispose();
                    Environment.Exit(0);
                }

                result = response.Where(x => x.Type == ActivityTypes.Message).ToList();
                if (result != null && result.Any())
                {
                    return result;
                }

                Thread.Sleep(1000);
            } while (response != null && response.Any());

            return new List<Microsoft.Bot.Connector.DirectLine.Activity>();
        }

        private static string BotReplyAsAPIResponse(List<Microsoft.Bot.Connector.DirectLine.Activity> responses)
        {
            string responseStr = "";
            responses?.ForEach(responseActivity =>
            {
                // responseActivity is standard Microsoft.Bot.Connector.DirectLine.Activity
                // See https://github.com/Microsoft/botframework-sdk/blob/master/specs/botframework-activity/botframework-activity.md for reference
                // Showing examples of Text & SuggestedActions in response payload
                Console.WriteLine(responseActivity.Text);
                if (!string.IsNullOrEmpty(responseActivity.Text))
                {
                    responseStr = responseStr + string.Join(Environment.NewLine, responseActivity.Text);
                }

                if (responseActivity.SuggestedActions != null && responseActivity.SuggestedActions.Actions != null)
                {
                    var options = responseActivity.SuggestedActions?.Actions?.Select(a => a.Title).ToList();
                    responseStr = responseStr + $"\t{string.Join(" | ", options)}";
                }
            });

            return responseStr;
        }

        public void Dispose()
        {
            directLineClient.Dispose();
        }
    }
}
