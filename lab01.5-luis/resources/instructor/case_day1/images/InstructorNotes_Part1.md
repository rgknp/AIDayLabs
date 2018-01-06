## Day 1 Instructor Notes

### How to conduct the session
* Give the students about 3 minutes to read the case and assignment
* Split the class into groups of about five. There should be an even number of groups (this will probably take about 2 minutes)
* Give the class 25 minutes to discuss within their groups
* Give the class 10 minutes to talk about their results with another group
* Take whatever time is left to show them some of the ideas below, and asking if anyone had other ideas
> Ideally, there would be one proctor per every two groups, to facilitate questions and provide insights if needed

### Potential answers to the questions

**Which features from Azure Search should you take advantage of?**
* Geospatial - maybe they want to know what's closest to them, or they only want to know about items in stock at a specific store
* Boosting - you may want to return higher-profit bikes or items in your list when they search, or maybe you want use ranking and go from frequently purchased to not
* Facets - maybe they want results in different areas, e.g. bikes, clothing, gloves, etc.
* Regularly scheduled indexer for Azure SQL the options change depending on what's in stock
* Monitoring, reporting and analyses to determine what is asked about, maybe things asked about more, you keep more and similar products in stock.


**What might your main intents in LUIS be?**
* Greeting
* Help menu
* Main menu / Start over
* Speak with an operator
* Search products
* Order products
* Check out
* Goodbye

**What are some tasks you'll have to complete to create an efficient and functional calling bot?**
* Call the Bing Speech API to get text-to-speech and speech-to-text
* Incorporate LUIS and call the intents
* Incorporate Search and call the service to search Azure SQL
* Implement a Regex structure to minimize hits to LUIS
* Might also be able to take advantage of FormFlow
* Set up the Skype channel

**Are there any additional Cognitive Services that you think the bot would benefit from?**  
* Custom Speech Service - over come recognition barriers like speaking style, background, noise, and vocabulary
* Speaker Recognition API - you could authenticate/identify return customers
* Translator Speech API - give the user the option to speak in different languages
* Recommendations API - predict and recommend items customers want

### Additional Information
Here's a [Learning Path](https://github.com/amthomas46/LearningPaths/blob/master/Developer/Learning%20Path%20-%20Interactive%20Voice%20Response%20Bot.md) I created if you want to know a little more about what goes into creating an Interactive Voice Response Bot.

This case was modified from this [Cortana Intelligence Solution](https://gallery.cortanaintelligence.com/Solution/Interactive-Voice-Response-Bot).