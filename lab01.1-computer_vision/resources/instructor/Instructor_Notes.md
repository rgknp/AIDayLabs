# Instructor Notes: Intro to the Bootcamp and Lab 1.1

**This document serves to provide some (hopefully) helpful notes and tips with regards to presenting these materials/labs**

## Cognitive Services and Lab 1.1 intro (recommended time 1.5 hours)
We had two people presenting this, and it worked really nicely to have a back-and-forth here. You want to pumpy people up and get them excited, having two exciting and engaging people helps.

*	Intro and Welcome to the bootcamp [~30 minutes] – basically a walkthrough of the schedule document and introductions
    *	Welcome 
        *    Poll of who is here, where they’re from, what their roles are, what they’re looking for from this session (redelivery, learning, ?)
        *   Introduce our team
    *	Goals
        *	Talk through some of the goals listed 
    *	Prerequisites
        *	Poll about the prerequisites and experience levels
        *	Explain them a little bit
        *	If we find that many people have not done the prereqs (1_Setup), after the intro and welcome, it may be a good idea to give people 5 minutes to start deploying the DSVM
    *	Agenda and logistics
        *	We’ll give some breaks, but during the labs, feel free to get up and use the restroom/get coffee as needed
        *	Go through the agenda
    *	Materials
        *	Show them where the materials are
        *   Stress how we want people to work together, use your neighbor first, etc  

*   Cognitive Services [~30 minutes]
    *	Open discussion - 
        *	What are cognitive services
        *   Ask the room - who has implemented, what are they, what do they think of them, how easy are they to use and access, are they flexible, are there options for customization?
    *	Intro to Cognitive Services – 
        *	Anna to start by leading discussion on why we might want to use cognitive services, and open discussion described above
        *	Chris to give intro about some of them
        *	Anna to talk a little more above the various cognitive services, homing in on the vision ones and computer vision
        *	Chris to drill in to custom vision 
    *	Black box vs customizable
    *	Use cases for cognitive services
        *	Maybe poll the class
        *	Explain some of the use cases – maybe share one of the following three videos
            *	Seeing AI https://www.bing.com/videos/search?q=cognitive+services+use+cases&&view=detail&mid=BF458FF95EEEBDFC6D75BF458FF95EEEBDFC6D75&FORM=VRDGAR 
            *	Uber use case https://azure.microsoft.com/en-us/resources/videos/how-uber-is-using-driver-selfies-to-enhance-security-powered-by-microsoft-cognitive-services/ 
            *	Litware insurance bot - https://www.youtube.com/watch?v=DfGccsNffdQ – no sound in this video so will need to talk through it
        *	Data collection
        *	Kiosk - Uber
        *	Accessibility – Seeing AI

*   Lab 1.1 Setup [~20 minutes]
    *	Set up the lab 
        *	Go through the architecture and use case and associated tasks 
        *	Demo to them
        *	Include a very brief tour of Visual Studio – maybe leave this out 
        *	Open it up for more discussion – at the end of the lab if there is time
    *	Questions
    *	Thoughts
    *	Experiences
    *	Challenges
	
## Known issue with RDP connection for the Lab

With the release of the March 2018 Security bulletin, there was a fix that addressed a CredSSP, “Remote Code Execution” vulnerability (CVE-2018-0886) which could impact RDP connections. 

*Symptoms*
1.       The VM screenshot shows the OS fully loaded and waiting for the credentials
2.       If you try to RDP the VM either internally or externally, you'll get the message: 
						
						An authentication error has occurred.
						The function requested is not supported. 
						
*Resolution/ Fix*
Ensure both client & server side have latest patch installed so that RDP can be established in a secure way by using one of the following methods

Method 1
1.	Press Win + R and type gpedit.msc in Run Command
2.	Browse to Computer Configuration > Administrative Templates  > System > Credentials Delegation in the left pane
3.	Change the Encryption Oracle Remediation policy to Enabled if it is not enable already
4.	And change Protection Level to Vulnerable

Method 2

1.	Press Win  and type CMD.
2.	Right click Command Prompt and click Run as Administrator, in the User Access Control window, click Yes.
3.	Type the following code into the window and press ENTER after the code
			REG  ADD HKLM\Software\Microsoft\Windows\CurrentVersion\Policies\System\CredSSP\Parameters\ /v AllowEncryptionOracle /t REG_DWORD /d 2
4.	Type the following code into the window and press ENTER after the code
			gpupdate /force
			
check the RCP connection

For more detailed steps, you can refer this blog here:
https://blogs.technet.microsoft.com/mckittrick/unable-to-rdp-to-virtual-machine-credssp-encryption-oracle-remediation/ 

			
## Lab 1.1 Tips
* The meat of this lab is within 2_ImageProcessor, majority of people will spend the most time here
       * Encourage people to attempt the free form coding sections before peeking at the solution file, but make sure they know it is there when they get stuck
    * If people get a key error in 3_TestCLI, pay attention to what service is returning the error. If it is Computer Vision, make sure they have put it in a location that the service is available (at time of this content, West US was the way to go)
    * There is no set solution for the challenge. This is intentional. We refrain from answering too many questions about this, because there are several ways to achieve it and our focus should be on those who have not completed the challenge yet. 
      * If students are confused because the package is not within ProjectOxford, give them a tip to check out the Microsoft.Translator NuGet package.
      * The general idea is for them to create an additional service helper and then translate the results from the Computer Vision API call
    * If all the students finish 3_TestCLI early, move on to the Custom Vision intro before lunch. Utilize all extra time that you are given, you never know where students will have more questions or difficulties, and the schedule is meant to be flexible.

## Known issue with RDP connection
