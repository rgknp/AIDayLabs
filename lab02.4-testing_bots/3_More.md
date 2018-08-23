## 3_More:

There are lots of other tools you can use when developing bots. We can't cover all of them, but we wanted to include a few topic introductions that can assist you when you're testing/deving bots:  

1. Unit testing bots
2. Bot analytics
3. Bot Builder tools

> Note: This is not a lab but an exercise to gain awareness and learn about related tools.

## 3.1: Unit Testing Bots

Writing code using Microsoft Bot Framework is fun and exciting. But before rushing to code bots, you need to think about unit testing your code. Chatbots bring their own set of challenges to testing including testing across environments, integrating third party APIs, etc. Unit testing is a software testing method where individual units/components of a software application are tested.  

Unit tests can help:
- Verify functionality as you add it
- Validate your components in isolation
- People unfamiliar with your code verify they haven't broken it when they are working with it  


As you (hopefully) noticed in lab02.2, every time you add functionality, you're increasing the complexity of the bot. We suggest that you get happy with one level (test using the emulator and the tools we have/will discuss), before you move on to the next. This will allow you to catch errors as they appear, as opposed to trying to find the issue in a complete bot later. So we try to isolate and fix the lowest level issues before adding additional complexity.  

We encourage you to utilize mock elements. Review [this page on testing bots that contains best practices and other references](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-testing-debugging?view=azure-bot-service-4.0).  

## 3.2: Bot analytics  

Bot analytics is an extension of Application Insights and is specific to Azure Bot Service. In order to access the conversation-level reporting on user, message, and channel data, you will need to have Application Insights turned on for your bot. You can read more about [getting bot analytics set up and what it provides here](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-analytics?view=azure-bot-service-4.0).  

While working with customers, one team identified the need for an easy-to-use tool for visualizing the service's data. They built a solution that supports various scenarios (not just related to bots) and creates a dashboard framework. You can read about [the project here](https://www.microsoft.com/developerblog/2017/09/26/custom-analytics-dashboard-application-insights/) and access the [complete code/instructions for implementation/customization here](https://github.com/Azure/ibex-dashboard).

## 3.3: Bot Builder tools

[Bot Builder tools](https://github.com/Microsoft/botbuilder-tools) are designed to cover end-to-end bot development workflow. Currently, they include the following tools that you should be aware of:  

- [Chatdown](https://github.com/Microsoft/botbuilder-tools/tree/master/Chatdown): Prototype mock conversations in markdown and convert the markdown to transcripts you can load and view in the new V4 Bot Framework Emulator
- [MSBot](https://github.com/Microsoft/botbuilder-tools/tree/master/MSBot): Create and manage connected services in your bot configuration file
- [LUDown](https://github.com/Microsoft/botbuilder-tools/tree/master/Ludown): Build LUIS language understanding models using markdown files
- [LUIS](https://github.com/Microsoft/botbuilder-tools/tree/master/LUIS): 
Create and manage your LUIS.ai applications
- [QnAMaker](https://github.com/Microsoft/botbuilder-tools/tree/master/QnAMaker): Create and manage QnAMaker.ai Knowledge Bases
- [Dispatch](https://github.com/Microsoft/botbuilder-tools/tree/master/Dispatch): Build language models allowing you to dispatch between disparate components (such as QnA, LUIS and custom code)
- [LUISGen](https://github.com/Microsoft/botbuilder-tools/tree/master/LUISGen): Autogenerate backing C#/Typescript classesfor your LUIS intents and entities  

We highly recommend that you read what these tools can do carefully, as they may be very useful in your bot-related projects.  

Also, you can check out an [example of an end-to-end bot development workflow using botbuilder-tools](https://aka.ms/BotBuilderLocalDev).


Back to [README](./0_README.md)
