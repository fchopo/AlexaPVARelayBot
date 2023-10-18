using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using Activity = Microsoft.Bot.Connector.DirectLine.Activity;

namespace MyPVABot.Helpers.DirectLine
{
    public interface DirectLineClientBase : IDisposable
    {
        public Task SendActivityAsync(Activity activity, CancellationToken cancellationToken);

        public Task<List<Activity>> ReceiveActivitiesAsync(CancellationToken cancellationToken);

        public void Dispose();
    }
}
