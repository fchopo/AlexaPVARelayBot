﻿using Microsoft.VisualBasic;
using MyPVABot.Helpers;
using System;
using System.Linq;

namespace MyPVABot.Models.DirectLine
{
    public class DirectLineOptions
    {
        private readonly Uri _botUrl;
        private readonly Uri _regionalEndpoint;
        private readonly string _botId;

        public Uri BotUrl { get { return _botUrl; } }
        public Uri RegionalEndpoint { get { return _regionalEndpoint; } }
        public string BotId { get { return _botId; } }

        public DirectLineOptions(string botUrl, Uri regionalEndpoint)
        {
            _regionalEndpoint = regionalEndpoint;
            _botUrl = new Uri(botUrl);
            _botId = GetBotNameFromUri(_botUrl);
        }

        private static string GetBotNameFromUri(Uri u)
        {
            var host = u.AbsolutePath;
            var parts = host.Split('/').ToList();
            int index = parts.IndexOf("botsbyschema");
            return ((index + 1) > 0) ? parts[index + 1] : Helpers.Constants.InvalidBotUri;
        }
    }
}
