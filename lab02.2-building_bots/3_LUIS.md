## 3_LUIS:
Estimated Time: 10-15 minutes

Our bot is now capable of taking in a user's input, calling Azure Search, and returning the results in a carousel of Hero cards. Unfortunately, our bot's communication skills are brittle. One typo, or a rephrasing of words, and the bot will not understand. This can cause frustration for the user. We can greatly increase the bot's conversation abilities by enabling it to understand natural language with the LUIS model we built yesterday in "lab01.5-luis."  

We will have to update our bot in order to use LUIS.  We can do this by modifying "Startup.cs" and "RootTopic.cs."

### Lab 3.1: Adding LUIS to Startup.cs

Open "Startup.cs" and find where you added middleware to use the RegEx recognizer middleware. Since we want to call LUIS **after** we call RegEx, we'll put the LUIS recognizer middleware below. Add the following under the comment "Add LUIS ability below":
```csharp
                middleware.Add(new LuisRecognizerMiddleware(
                    new LuisModel("luisAppId", "subscriptionId", new Uri("luisModelBaseUrl"))));
```
Use the app ID, subscription ID, and base URI for your LUIS model. The base URI will be "https://region.api.cognitive.microsoft.com/luis/v2.0/apps/", where region is the region associated with the key you are using. Some examples of regions are, `westus`, `westcentralus`, `eastus2`, and `southeastasia`.  

You can find your base URL by logging into www.luis.ai, going to the **Publish** tab, and looking at the **Endpoint** column under **Resources and Keys**. The base URL is the portion of the **Endpoint URL** before the subscription ID and other parameters.  

**Hint**: The LUIS App ID will have hyphens in it, and the LUIS subscription key will not.  

### Lab 3.2: Adding LUIS to RootDialog

Open "PictureBot.cs." There's no need for us to add anything to initial greeting, because regardless of user input, we want to greet the user when the conversation starts.  

In RootDialog, we do want to start by trying Regex, so we'll leave most of that. However, if Regex doesn't find an intent, we want the `default` action to be different. That's when we want to call LUIS.  

Replace:
```csharp
                        default:
                            // respond that you don't understand
                            await RootResponses.ReplyWithConfused(dc.Context);
                            break;
```
With:
```csharp
                        default:
                        // adding app logic when Regex doesn't find an intent - consult LUIS
                            var result = dc.Context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);
                            var topIntent = result?.GetTopScoringIntent();

                            switch ((topIntent != null) ? topIntent.Value.intent : null)
                            {
                                case null:
                                    // Add app logic when there is no result.
                                    await RootResponses.ReplyWithConfused(dc.Context);
                                    break;
                                case "None":
                                    await RootResponses.ReplyWithConfused(dc.Context);
                                    await RootResponses.ReplyWithLuisScore(dc.Context, topIntent.Value.intent, topIntent.Value.score);
                                    break;
                                case "Greeting":
                                    await RootResponses.ReplyWithGreeting(dc.Context);
                                    await RootResponses.ReplyWithHelp(dc.Context);
                                    await RootResponses.ReplyWithLuisScore(dc.Context, topIntent.Value.intent, topIntent.Value.score);
                                    break;
                                case "OrderPic":
                                    await RootResponses.ReplyWithOrderConfirmation(dc.Context);
                                    await RootResponses.ReplyWithLuisScore(dc.Context, topIntent.Value.intent, topIntent.Value.score);
                                    break;
                                case "SharePic":
                                    await RootResponses.ReplyWithShareConfirmation(dc.Context);
                                    await RootResponses.ReplyWithLuisScore(dc.Context, topIntent.Value.intent, topIntent.Value.score);
                                    break;
                                case "SearchPics":
                                    // Check if LUIS has identified the search term that we should look for.  
                                    var entity = result?.Entities;
                                    var obj = JObject.Parse(JsonConvert.SerializeObject(entity)).SelectToken("facet");
                                    // if no entities are picked up on by LUIS, go through SearchDialog
                                    if (obj == null)
                                    {
                                        await dc.Begin(SearchDialog.Id);
                                        await RootResponses.ReplyWithLuisScore(dc.Context, topIntent.Value.intent, topIntent.Value.score);
                                    }
                                    // if entities are picked up by LUIS, skip SearchDialog and process the search
                                    else
                                    {
                                        var facet = obj.ToString().Replace("\"", "").Trim(']', '[', ' ');

                                        await RootResponses.ReplyWithLuisScore(dc.Context, topIntent.Value.intent, topIntent.Value.score);
                                        await SearchResponses.ReplyWithSearchConfirmation(dc.Context, facet);
                                        await StartAsync(dc.Context, facet);
                                        break;
                                    }
                                    break;
                                default:
                                    await RootResponses.ReplyWithConfused(dc.Context);
                                    break;
                            }
                            break;
```
Let's briefly go through what we're doing in the new code additions. First, instead of responding saying we don't understand, we're going to call LUIS. So we call LUIS using the LUIS Recognizer Middleware, and we store the Top Intent in a variable. We then use `switch` to respond in different ways, depending on which intent is picked up. This is almost identical to what we did with Regex.  

> Note: If you named your intents differently in LUIS than instructed in "lab01.5-luis", you need to modify the `case` statements accordingly.  

Another thing to note is that after every response that called LUIS, we're adding the LUIS intent value and score. The reason is just to show you when LUIS is being called as opposed to Regex (you would remove these responses from the final product, but it's a good indicator for us as we test the bot).  

Bring your attention to `case "SearchPics"`. Here, we check if LUIS also returned an entity, specifically the "facet" entity. If LUIS doesn't find the "facet," we take the user through the search topic, so we can determine what they want to search for and give them the results.  

If LUIS does determine a "facet" entity from the utterance, we don't want to take the users through the whole search dialog. We want to be efficient so the user has a good experience. So we'll go ahead and process their search request. `StartAsync` does just that.  

At the bottom of the class, but still within the class, add the following:
```csharp
public static async Task StartAsync(ITurnContext context, string searchText)
        {
            ISearchIndexClient indexClientForQueries = CreateSearchIndexClient();
            // For more examples of calling search with SearchParameters, see
            // https://github.com/Azure-Samples/search-dotnet-getting-started/blob/master/DotNetHowTo/DotNetHowTo/Program.cs.  
            // Call the search service and store the results
            DocumentSearchResult results = await indexClientForQueries.Documents.SearchAsync(searchText);
            await SendResultsAsync(context, searchText, results);
        }

        public static async Task SendResultsAsync(ITurnContext context, string searchText, DocumentSearchResult results)
        {
            IMessageActivity activity = context.Activity.CreateReply();
            // if the search returns no results
            if (results.Results.Count == 0)
            {
                await SearchResponses.ReplyWithNoResults(context, searchText);
            }
            else // this means there was at least one hit for the search
            {
                // create the response with the result(s) and send to the user
                SearchHitStyler searchHitStyler = new SearchHitStyler();
                searchHitStyler.Apply(
                    ref activity,
                    "Here are the results that I found:",
                    results.Results.Select(r => ImageMapper.ToSearchHit(r)).ToList().AsReadOnly());

                await context.SendActivity(activity);
            }
        }

        public static ISearchIndexClient CreateSearchIndexClient()
        {
            // Configure the search service and establish a connection, call it in StartAsync()
            // replace "YourSearchServiceName" and "YourSearchServiceKey" with your search service values
            string searchServiceName = "YourSearchServiceName";
            string queryApiKey = "YourSearchServiceKey";
            string indexName = "images";
            // if you named your index "images" as instructed, you do not need to change this value

            SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, indexName, new SearchCredentials(queryApiKey));
            return indexClient;
        }
``` 
This code should look very familiar to you. It's quite similar to the `StartAsync` method in the search dialog.  

Hit F5 to run the app. In the Bot Emulator, try sending the bots different ways of searching pictures. What happens when you say "search pics" or "send me pictures of water"? Try some other ways of searching, sharing and ordering pictures.  

If you have extra time, see if there are things LUIS isn't picking up on that you expected it to. Maybe now is a good time to go to luis.ai, [review your endpoint utterances](https://docs.microsoft.com/en-us/azure/cognitive-services/LUIS/label-suggested-utterances), and retrain/republish your model. 


> Fun Aside: Reviewing the endpoint utterances can be extremely powerful.  LUIS makes smart decisions about which utterances to surface.  It chooses the ones that will help it improve the most to have manually labeled by a human-in-the-loop.  For example, if the LUIS model predicted that a given utterance mapped to Intent1 with 47% confidence and predicted that it mapped to Intent2 with 48% confidence, that is a strong candidate to surface to a human to manually map, since the model is very close between two intents.  


**Extra credit (to complete later):** Create and configure a "web.config" or "appsettings.json" file to store your search service information. Next, change the code in RootDialog.cs and SearchDialog.cs to call the settings in web.config, so you don't have to enter them twice.

**Extra credit (to complete later)**: Create a process for ordering prints with the bot using dialogs, responses, and models.  Your bot will need to collect the following information: Photo size (8x10, 5x7, wallet, etc.), number of prints, glossy or matte finish, user's phone number, and user's email. The bot will then want to send you a confirmation before submitting the request.


Get stuck? You can find the solution for this lab under [resources/code/FinishedPictureBot-Part3](./resources/code/FinishedPictureBot-Part3).


### Continue to [4_Publish_and_Register](./4_Publish_and_Register.md)  
Back to [README](./0_README.md)
