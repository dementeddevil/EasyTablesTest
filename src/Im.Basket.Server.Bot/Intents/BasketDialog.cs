﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace Im.Basket.Server.Bot.Intents
{
    [Serializable]
    public class BasketDialog : LuisDialog<LuisResult>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I do not understand. Try asking me for basket help.");
            context.Done(true);
        }
    }
}