# Setting up for the bootcamp

The following steps will get us up and running for the bootcamp. These activities take about 2.5 hours in total and should be completed **prior to attending** the bootcamp. Failure to do so will result in falling behind during the workshop and divert time and attention from the material covered throughout the bootcamp.
 
##  What we will need for this workshop: 

 -  A Microsoft Azure account where we can create resources, including Application Insights. This could be an organization account, an MSDN subscription account, a Trial Account, or a work account.
 -  A Microsoft Azure Machine Learning Experimentation and Model Management Account.
 -  A Windows laptop on which we can install software **OR** any machine with **Remote Desktop Connection** so that we can access a remote Windows Data Science Virtual Machine (Note: a **v3** type VM is required to leverage Docker if we chose an Azure DSVM, and this is officially not yet supported).

##  Setting up the environment 

For the remainder of this lab, we will be working exclusively on the DSVM. So log into the DSVM and then run through the steps outlined here.

1. Launch Docker for Windows and while waiting for Docker to start running, open the Azure Machine Learning Workbench. When prompted to authenticate using an Azure account, please do so.
2. Click on initials at the bottom-left corner of the Workbench and make sure that we are using the correct account (if you already have a Model Management account, otherwise we will create one later).
3. Go to **File > Configure Project IDE** and name the IDE `Code` with the following path `C:\Program Files\Microsoft VS Code\Code.exe`. This will allow us to open the entire project in Visual Studio Code, which is our editor of choice for this lab.
4. Go to **File > Open Project (VSCode)** to open the project in Visual Studio Code. It is not necessary to use Code to make edit our course files but it is much more convenient. We will return to Code when we need to make changes to the existing scripts.
5. We now log into the Azure CLI using our Azure account. Return to the Workbench and go to **File > Open Command Prompt**. Check that the Azure CLI is installed on the DSVM by typing `az -h`. Now type `az login` and copy the access code. In Firefox open a **private tab** using **CTRL+SHIFT+P** then enter the URL `aka.ms/devicelogin` and when prompted, paste in the access code. Next, authenticate using an Azure account.
6. We now set the Azure CLI to use the right Azure account. From the command prompt, enter `az account list –o table` to see available accounts. Then copy the subscription ID from the Azure account used to create an AML Workbench account and type `az account set –s <SUBSCRIPTION_ID>`, replacing `<SUBSCRIPTION_ID>` with the account ID.
