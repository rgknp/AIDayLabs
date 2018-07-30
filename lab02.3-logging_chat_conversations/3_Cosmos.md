## 3_Cosmos:
Estimated Time: 20-25 minutes

## Using Middleware to Log Data to Cosmos DB

In this last lab, we'll work through creating custom middleware to intercept and log messages as part of the middlware pipeline.  

Specifically, we'll set up a Cosmos DB instance, and create middleware to store "entries" as users interact with the bot. Each entry will include a date/time stamp, the message the user sent, and the reply/replies that the bot sent.  

### Lab 3.1: Setup  
There are a few things we need to configure before we connect our PictureBot to Cosmos DB:  

- Follow [Step 1 of this QuickStart](https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-get-started) to create an Azure Cosmos DB account.
    - Be sure to store the URI and Key, as you will need it shortly.
- Within our PictureBot solution, there is a NuGet package we need to work with our Cosmos DB accounts. Add the `Microsoft.Azure.DocumentDb.Core` package. 



### Lab 3.2: Creating the Middleware  

We've talked a little about Middleware throughout the other bot-related labs. However, spend a few moments to [review the basics of middleware here](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-middleware?view=azure-bot-service-4.0).  

In this section, we will create some custom middleware so we can store data in Cosmos DB.  

First, under the Middleware folder, add a new class "CosmosMiddleware.cs".  

Replace the using statements with the following:  
```csharp
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
```
We've added references to a few new libraries, particularly:
- Newtonsoft.Json: This will help us parse the JSON going back and forth to our database.
- Microsoft.Azure.Documents, Microsoft.Azure.Documents.Client, and Microsoft.Azure.Documents.Linq: These are for communicating with our database.  

Additionally, every piece of middleware implements `IMiddleware`, requiring us to define the appropriate handlers for middleware to work properly. This means you'll need to change `public class CosmosMiddleware` to `public class CosmosMiddleware : IMiddleware`.  

 At the top of the class, we need to add local variables for manipulating our database and a class to store the information we want to log. That information class, which we called "Log", defines what the JSON properties associated with each member will be named. We'll return to this later. For now, add and review the following to the `CosmosMiddleware : IMiddleware` class:
 ```csharp
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
```

The next thing we need to do is authenticate and connect to our Cosmos DB service. Below the `Log` class, include the following:
```csharp
// Cosmos (Document) Client
public CosmosMiddleware()
{
    CosmosUri = "YourCosmosUriHere";
    CosmosKey = "YourCosmosKeyHere";
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
```
Be sure to add your Cosmos DB URI and Key to the code. With a neighbor, discuss line by line what is happening here. The comments should provide hints.  

Notice that we dispose of the `DocumentClient` - this is a best practice, and the destructor takes care of that for us.  

So what exactly are we doing here?  

Our constructor creates the `DocumentClient`, which allows us to read and write from our database. We also make sure that a database and collection are set up.  

Creation of our database depends on attempting to do a basic read of first our database, then the collection within that database. If we hit a `NotFound` exception, we catch that and create the database or collection instead of throwing it up the stack. In most cases these will already be created, but for this example, this allows us to not worry about the creation of that database before first run. In the library, the database and collection are split into two functions for simplicity, but they have been combined in the code above.  

The last helper function we will define will read from our database and return the most recent specified number of records. (It's worth noting that there are better database practices for retrieving data than we use here, particularly when your data store is significantly larger, but for this lab it is fine.)  

Review the code below. What does the method allow us to do as a user interacting with the bot?  

After you understand the following code, add it below your existing code within the class:
```csharp
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
```

The last thing we have to do in our middleware is call the `OnTurn()` method which will handle the rest of the work. We only want to log when the current activity is a message, which we check for both before and after calling `next()`.  

We're going to break this task into three parts.  

The first thing we check is if this is our special case message, where the user is asking for the most recent history. If so, we call read from our database to get the most recent records, and send that to the conversation. Since that completely handles the current activity, we short circuit the pipeline instead of passing on execution.  

Review the following code and add it below the `ReadFromDatabase` helper function you just created:
```csharp
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
```
Second, to get the response that the bot sends, we create a handler every time we receive a message to grab those responses, which can be seen in the lambda we'll give to `OnSendActivity()`. It builds a string to collect all the messages sent through `SendActivity()` for this context object. Review and add the next chunk of the method:
```csharp
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
```
Third and finally, we wait for the execution to return up the pipeline from `next()`, when we'll assemble our log data and write it out to the database.  

Revhiew the following code to confirm you agree. Then, add it as the final chunk of the `OnTurn()` method:
```csharp
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
```


> [Read here to understand what the requirements for custom middleware are](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-create-middleware?view=azure-bot-service-4.0&tabs=csaddmiddleware%2Ccsetagoverwrite%2Ccsmiddlewareshortcircuit%2Ccsfallback).  





### Lab 3.3: Adding the Middleware and Results  
Navigate to the Startup class and add `using PictureBot.Middleware;` to your list of using statements. Quick test - why did we not need this using statement to access our RegEx middleware?

Now that we've created our middleware, we have to call it from the `ConfigureServices` method within our Startup class (with the rest of our middleware).  

Since middleware is executed in the order it apepars within `ConfigureServices`, we will add it after our middleware for `UserState` and `ConversationState`, but before our `RegExpRecognizerMiddleware`. Do you know why?  

Hint: it has something to do with if a user requests the history.  

Add the one line required to add the CosmosMiddleware to the pipeline.  

**If you get stuck, you can find the solution code under resources > code.**  

Run the bot and have several conversations with it. Look in the Cosmos DB Data Explorer (within your Cosmos DB service in the Azure portal), and you should see your data in individual records. When examining one of the records, notice that the first three items are the three values of our log data, named as the string we specified at the beginning of this lab for their respective `JsonProperty`.  

This lab was modified from suggestions [from the documentation, where they walk through a different but similar example and best practices](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-cosmos-middleware?view=azure-bot-service-4.0&tabs=cs).  

Looking to do more with [Cosmos DB and .NET](https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnet-samples)?



## Extra credit

After finishing all the activities, can you configure logging each conversation (not just one turn message/reply) as a different entry in Cosmos DB?  
  
Back to [README](./0_README.md)
