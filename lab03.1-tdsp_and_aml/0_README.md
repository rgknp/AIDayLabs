# The Team Data Science Process using Azure Machine Learning

This hands-on lab guides you through using the [Team Data Science Process (TDSP)](https://docs.microsoft.com/en-us/azure/machine-learning/team-data-science-process/overview) using [Azure Machine Learning Services (AMLS)](https://docs.microsoft.com/en-us/azure/machine-learning/preview/overview-what-is-azure-ml) with the Azure Machine Learning Workbench. We will be using a [Customer Churn Analysis example](https://docs.microsoft.com/en-us/azure/machine-learning/preview/scenario-churn-prediction) throughout this workshop.

In this workshop, you will:

- Understand and use the TDSP to clearly define business goals and success criteria
- Understand how to use a code-repository system with the Azure Machine Learning Workbench using the TDSP structure
- Create an example environment
- Use the TDSP and AMLS for data acquisition and understanding
- Use the TDSP and AMLS for creating an experiment with a model and evaluation of models
- Use the TDSP and AMLS for deployment
- Use the TDSP and AMLS for project close-out and customer acceptance

The objective in this lab is not to cover any data science scenario, but to learn about data science as a process.

***NOTE:*** There are several pre-requisites for this course, including an understanding and implementation of: 
  *  Programming using an agile methodology
  *  Machine learning and data science
  *  Working with the Microsoft Azure Portal

There is a comprehensive Learning Path you can use to prepare for this course [located here](https://github.com/Azure/learnAnalytics-CreatingSolutionswiththeTeamDataScienceProcess-/blob/master/Instructions/Learning%20Path%20-%20Creating%20Solutions%20with%20the%20Team%20Data%20Science%20Process.md).

## Introduction to the TDSP

![Image](resources/docs/images/tdsp.png)

## 1. Business Understanding

In the [Business Understanding](https://docs.microsoft.com/en-us/azure/machine-learning/team-data-science-process/lifecycle-business-understanding) phase of the TDSP, you discover the questions that the organization would like answered from data. This is a group effort, involving the organization, the Data Science team, and the DevOps team along with other stakeholders. 

Your scenario is as follows: 

The Orange Telecom company in France is one of the largest operators of mobile and internet services in Europe and Africa and a global leader in corporate telecommunication services. They have 256 million customers worldwide. They have significant coverage in France, Spain, Belgium, Poland, Romania, Slovakia and Moldova, and a large presence Africa and the Middle East.
Customer churn is always an issue in any company. Orange would like to predict the propensity of customers to switch providers (churn), buy new products or services (appetency), or buy upgrades or add-ons proposed to them to make the sale more profitable (up-selling). In this effort, they think churn is the first thing they would like to focus on.

To create a solution, we use the Azure Machine Learning Services (AMLS) and the Azure Machine Learning Services Workbench (WB) in this lab. In general Azure Machine Learning is configured with these components:

![Azure Machine Learning Components](resources/docs/images/aml-architecture-1.png)

### Lab: Set up a generic TDSP Structure using the Azure Machine Learning Workbench

In this lab you'll set up your project's structure, conforming to the Team Data Science Process, using the Azure Machine Learning Workbench (or Workbench for short).

- [Open this link](https://docs.microsoft.com/en-us/azure/machine-learning/preview/how-to-use-tdsp-in-azure-ml), read from the top and complete the steps there. Do not complete the steps marked **"Next Steps"**. 
- [Review this link](https://github.com/Azure/Azure-TDSP-ProjectTemplate) and verify that you have the structure shown in the directory you specified. You will use this structure throughout this workshop.

### Lab: Use-case evaluation for Data Science questions

In this lab you'll evaluate a business scenario, and detail possible predictions, classifications, or other data science questions that you can begin to explore.

Read the scenario above carefully. You can optionally copy and paste the scenario text below into a new text file called `Business Understanding.md` in the `/docs` directory set up in the previous lab. After class discussion, you can enter the answers to the following questions in this document.

- Is this something that can be solved with machine learning?
- Which algorithm or family of algorithms could you use to answer your question?
- What data source(s) will you need to complete your prediction? 
- How will the users interact with the solution?

## 2. Data Acquisition and Understanding

The [Data Aquisition and Understanding](https://docs.microsoft.com/en-us/azure/machine-learning/team-data-science-process/lifecycle-data) phase of the TDSP you ingest or access data from various locations to answer the questions the organization is asking. In most cases, this data will be in multiple locations. 
Once the data is ingested into the system, you'll need to examine it to see what it holds. All data needs cleaning, so after the inspection phase, you'll replace missing values, add and change columns. You'll cover more extensive "data wrangling" tasks in other labs. 


### Lab: Ingest data from a local source

In this lab, we'll use a single file-based dataset to train our model. You will load the data set, inspect it, make a few changes, and then save the data wrangling steps in a Python package.

- Open [this reference](https://github.com/Azure/MachineLearningSamples-ChurnPrediction/blob/master/docs/DataPreparation.md) and follow the steps you see there.

## 3. Modeling

The [modeling](https://docs.microsoft.com/en-us/azure/machine-learning/team-data-science-process/lifecycle-modeling) phase of the Team Data Science Process involves creating one or many experiments, where each experiment uses a machine learning algorithm that sifts through the data and attempts to create a prediction for a data point we are interested in (in this case, whether a customer will churn or not). When the prediction is a categorical prediction (such as "Yes" the customer will churn or "No" the customer will not churn), we refer to the problem as a **classification** problem and sometimes refer to the prediction algorithm as a **classifier**.

- You'll begin by using the `sklearn` library to create two models, one using a Naïve Bayesian algorithm and the other using a Decision Tree algorithm to develop a churn classifier. We then do an evaluation of each model and compare the two models.
- Finally, after the experiments run, you'll score the models to select the best one.

An view of this process is here, shown on the *right* side of the Docker graphic: 

![Image](resources/docs/images/aml-architecture-3.png)


### Lab: Feature Engineering, Modeling, and Scoring

In this lab we'll use the same project you just created. You'll create your feature engineering file, run the model training, and create the final scores.

- [Navigate to this resource](https://github.com/Azure/MachineLearningSamples-ChurnPrediction/blob/master/docs/ModelingAndEvaluation.md), and complete the steps 1-4. 

## 4. Deployment

An view of this process is here, shown on the *left* side of the Docker graphic: 

![Image](resources/docs/images/aml-architecture-3.png)

The [deployment](https://docs.microsoft.com/en-us/azure/machine-learning/team-data-science-process/lifecycle-deployment) phase of the TDSP entails serving the model's predictions by creating an Application Programming Interface (API) or another mechanism, so that the model can be consumed in some production environment.

### Optional Lab: Deploy the solution using Containers, consume the results

In this lab you will deploy the solution locally, and optionally to Docker. **NOTE** This section takes quite some, so it's included here for completeness. The instructor will go over it with you.

- [Navigate to this resource](https://github.com/Azure/MachineLearningSamples-ChurnPrediction/blob/master/docs/ModelingAndEvaluation.md) and complete the steps 5-8. 

## 5. Customer Acceptance

The final step in the Team Data Science Process is [customer acceptance](https://docs.microsoft.com/en-us/azure/machine-learning/team-data-science-process/lifecycle-acceptance). Here you focus on ensuring that the model performed within acceptable time and accuracy rates, and also present your findings in a comprehensive project document.

### Lab: Review customer acceptance and close-out documentation

In this lab you will examine the final project close-out document. In production implementations, you and your team will create this document. [Navigate to this resource](https://github.com/Azure/MachineLearningSamples-TDSPUCIAdultIncome/blob/master/docs/deliverable_docs/ProjectReport.md) and evalaute the report you see there. This is what you would create as a deliverable for your project. Address the following questions:

- Is there additional information needed there?
- Are there items you would also include?
- How should this document be communicated?
- Are there security implications?

## Workshop Completion

In this workshop you learned how to:

- Understand and use the TDSP to clearly define business goals and success criteria
- Understand how to use a code-repository system with the Azure Machine Learning Workbench using the TDSP structure
- Create an example environment
- Use the TDSP and AMLS for data acquisition and understanding
- Use the TDSP and AMLS for creating an experiment with a model and evaluation of models
- Use the TDSP and AMLS for deployment
- Use the TDSP and AMLS for project close-out and customer acceptance
