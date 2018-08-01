using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace PictureBot.Middleware
{
    public class CosmosMiddleware : IMiddleware
    {
        // Local variables for manipulating our database
        private string CosmosUri;
        private string CosmosKey;
        private static readonly string Database = "BotData";
        private static readonly string Collection = "BotCollection";
        public DocumentClient docClient;

        // Class to store the information we want to log
        // This class defines what the JSON properties associated with each member will be named
        public class Log
        {
            [JsonProperty("Time")]
            public string Time;

            [JsonProperty("MessageReceived")]
            public string Message;

            [JsonProperty("ReplySent")]
            public string Reply;
        }
        // Cosmos (Document) Client
        public CosmosMiddleware()
        {
            CosmosUri = "YourCosmosUri";
            CosmosKey = "YourCosmosKey";
            docClient = new DocumentClient(new Uri(CosmosUri), CosmosKey);
            CreateDatabaseAndCollection().ConfigureAwait(false);
        }

        ~CosmosMiddleware()
        {
            docClient.Dispose();
        }

        // Initialize database connection        
        private async Task CreateDatabaseAndCollection()
        {
            try // first attempt to read the Database
            {
                await docClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(Database));
            }
            catch (DocumentClientException e)
            {
                // if the Database doesn't exist yet
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // create it with the Id we provided earlier
                    await docClient.CreateDatabaseAsync(new Database { Id = Database });
                }
                else
                {
                    throw;
                }
            }

            try // next, attempt to access the Collection
            {
                await docClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri
                    (Database, Collection));
            }
            catch (DocumentClientException e)
            {
                // if the Collection doesn't exist yet
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // create it with the Id we provided earlier
                    await docClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(Database),
                        new DocumentCollection { Id = Collection });
                }
                else
                {
                    throw;
                }
            }
        }

        // Read from database logic
        public async Task<string> ReadFromDatabase(int numberOfRecords)
        {
            var documents = docClient.CreateDocumentQuery<Log>(
                    UriFactory.CreateDocumentCollectionUri(Database, Collection))
                    .AsDocumentQuery();
            List<Log> messages = new List<Log>();
            while (documents.HasMoreResults)
            {
                messages.AddRange(await documents.ExecuteNextAsync<Log>());
            }

            // Create a sublist of messages containing the number of requested records.
            List<Log> messageSublist = messages.GetRange(messages.Count - numberOfRecords, numberOfRecords);

            string history = "";

            // Send the last 3 messages.
            foreach (Log logEntry in messageSublist)
            {
                history += ("Message was: " + logEntry.Message + " Reply was: " + logEntry.Reply + "  ");
            }

            return history;
        }

        // Define OnTurn
        public async Task OnTurn
            (ITurnContext context, MiddlewareSet.NextDelegate next)
        {
            string botReply = "";

            if (context.Activity.Type == ActivityTypes.Message)
            {
                if (context.Activity.Text == "history")
                {
                    // Read last 3 responses from the database, and short circuit future execution.
                    await context.SendActivity(await ReadFromDatabase(3));
                    return;
                }
            }
            // Create a send activity handler to grab all response activities 
            // from the activity list.
            context.OnSendActivities(async (activityContext, activityList, activityNext) =>
            {
                foreach (Activity activity in activityList)
                {
                    botReply += (activity.Text + " ");
                }
                return await activityNext();
            });
            // Pass execution on to the next layer in the pipeline.
            await next();

            // Save logs for each conversational exchange only.
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Build a log object to write to the database.
                var logData = new Log
                {
                    Time = DateTime.Now.ToString(),
                    Message = context.Activity.Text,
                    Reply = botReply
                };

                // Write our log to the database.
                try
                {
                    var document = await docClient.CreateDocumentAsync(UriFactory.
                        CreateDocumentCollectionUri(Database, Collection), logData);
                }
                catch (Exception ex)
                {
                    // More logic for what to do on a failed write can be added here
                    throw ex;
                }
            }
        }
    }

}
