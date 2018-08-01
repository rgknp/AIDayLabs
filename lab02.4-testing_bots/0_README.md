# Testing Bots with Microsoft Bot Framework

This hands-on lab guides you through some of the basics of testing bots.  

## Objectives 

This workshop demonstrates how you can perform testing using the Microsoft Bot Framework. After completing these labs, you should be able to:  

- Test your bots remotely using ngrok  
- Perform functional testing (using Direct Line)  
- Be aware of other tools for bot development/testing

## Prerequisites

#### Lab02.2-building_bots  
This lab starts from the assumption that you have built and published the bot from `lab02.2-building_bots`. It is recommended that you do that lab in order to be successful in the ones that follow. If you have not, reading carefully through all the exercises and looking at some of the code or using it in your own applications may be sufficient, depending on your needs. We'll also assume that you've completed `lab02.3-logging_chat_conversations`, but you should be able to complete the labs without completing the logging labs.  

#### Have access to Azure  
You will need to have access to portal and be able to create resources on Azure. We will not be providing Azure passes for this workshop.  

## Introduction
Writing code using Microsoft Bot Framework is fun and exciting. But before rushing to code bots that can make tea and send spaceships to Mars, you need to think about testing your code. This workshop demonstrates how you can:

1. Perform rapid development/testing using ngrok
2. Perform functional testing (using Direct Line).
3. Utilize other tools for bot development/testing

## Navigating the GitHub ##

There are several directories in the [resources](./resources) folder:

- **assets**, **instructor**: You can ignore these folders for the purposes of this lab.
- **code**: In here, we will have finished solutions for each lab. If you choose to use a finished solution, you will need to add keys for all Azure services (they have been removed).
	- **PictureBot-FinishedSolution-Ngrok**: No real code is added in this lab, but you can see how the .bot file changes.
	- **PictureBot-FinishedSolution-DirectLine**: Finished solution including the console application.


> You need Visual Studio to run these labs, but if you have already deployed a Windows Data Science Virtual Machine for one of the workshops, you could use Visual Studio within the DSVM.

## Collecting the Keys

Over the course of this lab, we will collect various keys. It is recommended that you save all of them in a text file, so you can easily access them throughout the workshop.

>_Keys_
>- Microsoft Bot Resource ID: 
>- Microsoft Bot AppId: 
>- Microsoft Bot AppPassword: 
>- Direct Line Secret key: 


## Navigating the Labs

This workshop has been broken down into five sections:
- [1_Ngrok](./1_Ngrok.md): The aim of this hands-on lab is to show how you can use ngrok to perform rapid development/testing.  
- [2_Direct_Line](./2_Direct_Line.md): This hands-on lab demonstrates how you can communicate directly with your bot from a custom client.   
- [3_More](./3_More.md): This read-only lab exposes you to other resources and tools for development and testing with bots.  


### Continue to [1_Ngrok](./1_Ngrok.md)

