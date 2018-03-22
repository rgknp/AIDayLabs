using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Core.Extensions;

namespace PictureBot.Models
{

        /// <summary>
        /// Object persisted as conversation state
        /// </summary>
        public class ConversationData : StoreItem
        {
            public ITopic ActiveTopic { get; set; }
        }

        /// <summary>
        /// Object persisted as user state 
        /// </summary>
        public class UserData : StoreItem
        {
            public SearchHit SearchHit { get; set; }
        }
    
}
