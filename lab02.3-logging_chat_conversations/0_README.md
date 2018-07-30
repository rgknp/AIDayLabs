---
layout: default
---

# Logging with Microsoft Bot Framework 

This hands-on lab guides you through enabling various logging scenarios for your bot solutions.  

## Objectives
This workshop demonstrates how you can perform logging using Microsoft Bot Framework and store aspects of chat conversations. After completing these labs, you should be able to:  

- Understand how to intercept and log message activities between bots and users.  
- Log utterances to either file storage or Azure storage.  
- Use middleware to be able to log conversations to Cosmos DB.  

> Note: for these labs, we will be using the v4 of the Microsoft Bot Framework SDK. If you would like to perform similar labs with the v3 SDK, refer [here](https://azure.github.io/learnAnalytics-AdvancedFeaturesforMicrosoftBotFramework/).

## Prerequisites  

#### Lab02.2-building_bots  
This lab starts from the assumption that you have built and published the bot from `lab02.2-building_bots`. It is recommended that you do that lab in order to be successful in the ones that follow. If you have not, reading carefully through all the exercises and looking at some of the code or using it in your own applications may be sufficient, depending on your needs.  

#### Have access to Azure  
You will need to have access to portal and be able to create resources on Azure. We will not be providing Azure passes for this workshop.  

## Introduction

### Looking back  
In the previous labs, we build an end-to-end scenario that allows you to pull in your own pictures, use Cognitive Services to find objects and people in the images, figure out how those people are feeling, and store all of that data into a NoSQL Store (CosmosDB). Then we used that NoSQL Store to populate an Azure Search index, and then build a Bot Framework bot using LUIS to allow easy, targeted querying.

### What next?

Unless your bot is logging the conversation data somewhere, the bot framework will not perform any logging for you automatically. This has privacy implications, and many bots simply can't allow that in their scenarios.  

In the advanced analytics space, there are plenty of uses for storing log converstaions. Having a corpus of chat conversations can allow developers to: 
1. Build question and answer engines specific to a domain.
2. Determine if a bot is responding in the expected manner.
3. Perform analysis on specific topics or products to identify trends.  

In the course of the following labs, we'll walk through how we can intercept messages and some of the various ways we might store the data.  


## Navigating the GitHub ##

There are several directories in the [resources](./resources) folder:

- **assets**, **instructor**: You can ignore these folders for the purposes of this lab.
- **code**: In here, we will have finished solutions for each lab. If you choose to use a finished solution, you will need to add keys for all Azure services (they have been removed).
	- **FinishedPictureBot-File**: Here there is the finished PictureBot.sln that uses temporary file storage.
	- **FinishedPictureBot-Azure**: Here there is the finished PictureBot.sln that packages and code necessary to store utterances in an Azure storage account.
	- **FinishedPictureBot-Cosmos**: Finally, here is the solution for storing conversation data in Cosmos DB.


> You need Visual Studio to run these labs, but if you have already deployed a Windows Data Science Virtual Machine for one of the workshops, you could use Visual Studio within the DSVM.

## Collecting the Keys

Over the course of this lab, we will collect various keys. It is recommended that you save all of them in a text file, so you can easily access them throughout the workshop.

>_Keys_
>- Bot Service App ID:
>- Bot Service App Password:
>- Azure Storage Account Connection String:
>- Cosmos DB URI:
>- Cosmos DB Key:


## Navigating the Labs

This workshop has been broken down into five sections:
- [1_File](./1_File.md): After using the Emulator to see activity in our published bot, we'll connect PictureBot to file storage and examine the contents.  
- [2_Azure](./2_Azure.md): Next, we'll set up our bot to use an Azure Storage Account to log the list of utterances that users are sending to our bot.  
- [3_Cosmos](./3_Cosmos.md): Finally, we'll configure logging so we are able to get a user's message and the reply together, and we'll set up storing that data in Cosmos DB using Middleware.  


### Continue to [1_File](./1_File.md)

## Extra credit

After finishing all the activities, can you configure logging each conversation (not just one turn message/reply) as a different entry in Cosmos DB?  

### Continue to [Lab 2.4 - Testing your bot](../lab02.4-testing_bots/0_README.md)
