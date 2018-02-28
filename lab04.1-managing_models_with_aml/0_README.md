# Manage Models with Azure Machine Learning and related services

This hands-on lab guides us through managing and retraining models using [Azure Machine Learning (AML)](https://docs.microsoft.com/en-us/azure/machine-learning/preview/overview-what-is-azure-ml). 

In this lab, we will:

- Understand model versioning in AML
- Track models
- Create Docker containers with the models and test them locally

We focus on the objectives above, not data science, machine learning or a difficult scenario.  

***NOTE:*** There are several pre-requisites for this course, including an understanding and implementation of: 

  - Programming using an Agile methodology
  - Machine Learning and Data Science
  - Working with the Microsoft Azure Portal

There is a comprehensive Learning Path we can use to prepare for this course [located here](https://github.com/Azure/learnAnalytics-CreatingSolutionswiththeTeamDataScienceProcess-/blob/master/Instructions/Learning%20Path%20-%20Creating%20Solutions%20with%20the%20Team%20Data%20Science%20Process.md).

## Azure Machine Learning Model Management

Here is the high-level architecture of an end-to-end solution with AML, which handles both the development and operationalization of a Machine Learning model. We should return to this chart as we run through this lab to see how all the pieces come together.

![](https://docs.microsoft.com/en-us/azure/machine-learning/preview/media/overview-general-concepts/hierarchy.png)

The three main Azure resources we will consume in this lab are as follows:

 - An **Experimentation account** contains workspaces, which is where our projects are sitting. When working in teams, we can add multiple users or "seats" to an experimentation account. To use the AML and run experiments, we must create an experimentation account.
 - A **Model Management account** is for managing models. Model management is essential to both model development and deployment. We use a model management account to register models and bundle models and code (including dependencies) into **manifests**. Manifests are in turn used to create Docker **images**, and those images in turn build containerized web services that run instances of our deployed application, locally or in the cloud.
 - An **Environment** denotes a particular computing resource that is used for deploying and managing models. It can be a local computer, a Linux VM on Azure, or a Kubernetes cluster running in Azure Container Service. A model hosted in a Docker container runs in these environments and is exposed as a REST API endpoint.

After this lab is finished, we will have a better idea of how to use AML in order to:

- Minimize the time and effort that goes into the iterative process of building and evaluating ML models
- Smooth out the transition of going from development to production for operationalizing ML models
- Get back to doing more data science and less administrative or devops types of tasks

### Running the modeling script in Docker

Our task in this section is to successfully estimate a model in a Docker container. To do so, we will need to start a new Workbench project based on an existing template and make a set of changes to the project in order to run it successfully. 

Open the Workbench and press **CTRL+N** to create a new project. Name the project `churn_prediction` and use the `Documents` folder as the project directory. Finally, in the box called `Search Project Templates`, type `churn` and select the template called `Customer Churn Prediction`. Press **Create** to create the project.

Go to **File > Open Project (Code)** to edit the project scripts using Code. Find the script `CATelcoCustomerChurnModelingWithoutDprep.py` and find the line where the data is read: `df = pd.read_csv('data/CATelcoCustomerChurnTrainingSample.csv')` to see how the raw data is read.

Go to `aml_config/docker.runconfig` and replace its content with 

```
ArgumentVector:
  - "$file"
Target: "docker"
EnvironmentVariables:
  "EXAMPLE_ENV_VAR": "Example Value"
Framework: "Python"
CondaDependenciesFile: "aml_config/conda_dependencies.yml"
SparkDependenciesFile: "aml_config/spark_dependencies.yml"
PrepareEnvironment: true
TrackedRun: true
```

On Code, go to **File > Save All** to save all the above changes. Then return to the Workbench and check to make sure the changes are visible here. We can check by clicking on the **Files** tab on the left pannel and opening one of the files we changed.

In order to run the experiment in a Docker container, we must prepare a Docker image. We will do so programatically by going to **File > Open Command Prompt** and typing:
 
 ```az ml experiment prepare -c docker```
 
 Notice all the changes that are happening as this command is running. This should take a few minutes.

We can now run our experiment in a Docker container by submitting the following command: 

```az ml experiment submit -c docker CATelcoCustomerChurnModelingWithoutDprep.py```

Alternatively, we can go to the **Project Dashboard**, select "docker" as the run configuration, select the `CATelcoCustomerChurnModelingWithoutDprep.py` script and click on the run button. In either case, we should be able to see a new job starting on the **Jobs** in the pannel on the right-hand side. Click on the finished job to see the **Run Properties** such as **Duration**. Notice under **Outputs** there are no objects, so the script did not create any artifacts. Click on the green **Completed** to see any results printed by the script, including the model accuracy. It is worth noting that the Azure CLI runs on both the CMD and Linux command line. To see this in action, from the Windows command prompt type `bash` to switch to a Linux command prompt and type:

```az ml experiment submit -c docker CATelcoCustomerChurnModelingWithoutDprep.py```

When finished, click on the job and notice the `model.pkl` entry in the **Run Properties** pane under **Outputs**. Select this output, download it and place it the project's root folder.

### Creating a web service out of the scoring script

Let's now see how we can create a scoring web service from the above model inside a docker image. There are multiple steps that go into doing that. We will be running commands from the command line, but we will also log into the Azure portal in order to see which resources are being created as we run various Azure CLI commands.

Enable the Azure Container Service and Azure Container Registry by making sure they are providers that are registered with your current subscription: 

```
az provider register -n Microsoft.ContainerService
az provider register -n Microsoft.ContainerRegistry
```

We can check the status of our registration by running:

```
az provider list --query "[?contains(namespace,'Container')]" -o table
```

`Microsoft.ContainerService` and `Microsoft.ContainerRegistry` should have `registrationState` of `Registered`.

Log into the Azure portal and find all the resources under the resource group. This should include an Experimentation and a Model Management account. Open the Model Management resource and click on **Model Management** icon on the top.

If we're doing this for the first time, then we need to set up an environment. We usually have a staging and a production environment. We can deploy our models to the staging environment to test them and then redeploy them to the production environment once we're happy with the result. To create a new environment run the following command after choosing a name for the staging environment. To use in production, we can provide the additional `--cluster` argument (covered in a later lab):

```
az ml env setup -l eastus2 -n <STAGING_ENVIRONMENT> -g <RESOURCE_GROUP>
```

We can look at all the environments under our subscription using `az ml env list -o table`. Creating the new environment takes about one minute, after which we can activate it and show it using this:

```
az ml env set -n <STAGING_ENVIRONMENT> -g <RESOURCE_GROUP>
az ml env show
```

We next set our Model Management account by running the following command. We can look up the Model Management account name from the Azure portal or by listing it:

```
az ml account modelmanagement list -o table
az ml account modelmanagement set -n <MODEL_MANAGEMENT_ACCOUNT> -g <RESOURCE_GROUP>
```

We are now finally ready to deploy our model as a web service. 

Generate the schema by first running: 

```python churn_schema_gen.py```

This will create a `service_schema.json` file that specifies the structure of the inputs that the scoring function `score.py` expects. Once we have this json file, we can then create a realtime scoring service in a single command by running: 

```
az ml service create realtime -n churnpred --model-file ./model.pkl -f score.py -r python -s service_schema.json 
```

The `az ml service create` command does four distinct steps:

1. It registers the model to facilitate versioning.
2. It creates a manifest used to build a docker image.
3. It builds a Docker image based on that manifest, and 
4. It initializes and runs that Docker image to provide the end-point for the prediction app. 

We can see these resources by going to the Azure portal and navigating to the Model Management resource, then click on **Model Management** in the blade of that resource.

![](./images/model-management-portal.jpg)

In the Model Management portal, we can view the resources that are created as the above command runs: the model, the manifest, the image, and the service. Click on each to view the resources.

![](./images/model-management-services.jpg)

Note that we can see the model, the manifest and the image on the Azure portal, but we can't see the service we created. This is because we ran `az ml env setup ...` *without* the `--cluster` argument, which means the service was created locally. This can be useful for the purpose of testing the service as we develop our application. In a future lab, the same service will be deployed remotely to Azure Container Service and will be visible from the Azure portal.

Return to the command line and test the service by running the example command given in the line `Usage for cmd: az ml service run realtime ...` which can be found in the output generated by the last command. As a result, the command should return `"0"`, which is the prediction made by the model (this model predicts whether someone churns (`"1"`) or doesn't not churn (`"0"`) based on the information provided). This is not the most convenient way to test the service but it has the advantange of being done directly from the command line. 

Note: If we run `az ml service run realtime ...` from `bash`, we need to change `"\\N\"` to `"\\\\N\"` because the backslash character has to be escaped using a second backslash character.

## Lab Completion

In this workshop we learned how to:

- Track models
- Create Docker containers with the models and test them locally
