# Instructor Notes: Custom Vision notes and Lab 1.2, 1.3 and 1.4

## Custom Vision intro and labs (total recommended time 1.5 hours)
We had two people presenting this, and it worked really nicely to have a back-and-forth here. You want to pumpy people up and get them excited, having two exciting and engaging people helps.

*	Intro to Custom Vision [~30 minutes] – basically a walkthrough of the schedule document and introductions
    *	Goals
        *	At the end of this session, your students should be able to:
            *	Create a Custom Vision project
            *	Upload and Tag images that are then trained
            *	Understand iterations and how to set a default iteration
            *	Test a trained model and perform a prediction with an independent image            
            
    *	Prerequisites
        *	The student should have an Azure Subscription
        *	A valid Microsoft account or an Azure Active Directory OrgID ("work or school account"), so you can sign into customvision.ai and get started.
        
    *	Agenda and logistics
        *	Create a Custom Vision project
        *   Classification and Object Detection
        *	Upload and Tag images into the Custom Vision project  
        *	Train a Custom Vision model
        *	Understand iterations and how to set a default iteration
        *	Test a trained model
        *	Retrieve a training and prediction key
        *	Perform a prediction with an independent image

        It is recommended to perform all of the above agenda through the customvision.ai website first and then show code to reinforce the action. Don't be afraid to experiment with different prediction images that have different angle and perspectives of the objects that you predicting, and focus on the probability output of the results asking the students why they think the probability is what it is

    *	Materials
        *	Show them where the materials are
        *   Stress how we want people to work together, use your neighbor first, etc 

## Demo information 

## Demo 1.2 Tips [~15 minutes]

    The files for the demo are located in the Resources\Instructor\Demo folder.
        *   You can run the solution file for the complete solutionfrom the Resources\Instructor\Demo\Solution folder.
        *   A partial solution is available that enables you to code while teaching in the Resources\Instructor\Demo\Starter folder.

    The Demo classifies cars that have been in an auto accident. There are two classifications:
        *   Dents - for cars that have a dent
        *   Write-off - for cars that are written off.

    Use case for this demo is the automation of handling car insurance claims. As the class if they can see any bias in the images used to train the solution.

    Answer. Cars that are classified with dents have images that have close ups of the dents on a car. Write-offs shows images with a full picture of a car. 
    Ask the class how they would address such bias?

## Demo 1.3 Tips [~15 minutes]

    Use the Solution folder of the repository and perform a walkthrough of the code that they will be creating as part of this lab

## Demo 1.4 Tips [0 minutes]

    There is no demo for this section

## Lab information

    There are three labs in total. For students who are new to this topic, advise them to complete lab 1.2 and 1.3. If they finsih early then they can start
    lab 1.4, but stress that they are not likely to finish this lab.
    For the more experience developer in Custom Vision, you could ask them to ignore lab 1.2 and 1.3, and spend 45 minutes on lab 1.4

## Lab 1.2 Tips [~15 minutes]
* The purpose of this lab is to focus on the following programatic elements of the .Net solution
    *	Creating a new project
    *	Make two tags in the new project
    *	Add some images to the tags
    *	Upload images both one at a time and as a batch
    *	Train the project 
    *	Make a prediction

    The README.md file for this lab contains the partial code answers by design. Encourage the students to explore and understand the code and work in groups to answer the questions. A solution file has been provided for those students who learn better by reading code.

## Lab 1.3 Tips [~15 minutes]
* The purpose of this lab is to focus on the following programatic elements of the .Net solution
    *	Creating a new project
    *   Instantiating the Object Detection Domain
    *	Define two tags in the new project
    *	Add some images to the tags
    *	Train the project 
    *	Make a prediction

    The README.md file for this lab contains the partial code answers by design. Encourage the students to explore and understand the code and work in groups to answer the questions. A solution file has been provided for those students who learn better by reading code.

## Lab 1.4 Tips [~45 minutes]
## This lab is a challenge exercise
* The purpose of this lab is to focus on the following programatic elements of the .Net solution
    *	Creating a new project
    *	Make two tags in the new project
    *	Add some images to the tags
    *	Upload images both one at a time and as a batch
    *	Train the project 
    *	Make a prediction
    *	retrieving and using a training key
    *	Upload images from a local disk into memory

    The README.md file for this lab DOES NOT contains the code answers by design. 
    This lab is recommended for experienced students are those that complete 1.2 and 1.3
    A solution file has been provided for those students who learn better by reading code.
    NOTE THAT STUDENTS MAY NOT FINISH THIS LAB

## Lab 1.2, 1.3 and 1.4 closedown [~15 minutes]
   *	Questions
   *	Thoughts
   *	Experiences
   *	Challenges


