# Deploying a scoring service to the Azure Container Service (AKS)

This hands-on lab guides us through deploying a Machine Learning scoring file to a remote environment using [Azure Machine Learning Services](https://docs.microsoft.com/en-us/azure/machine-learning/preview/overview-what-is-azure-ml) with Workbench. 

In this workshop, we will:
- Understand how to create a model file
- Generate a scoring script and schema file
- Prepare the scoring environment
- Deploy the model
- Run and update the real-time web service

***NOTE:*** There are several pre-requisites for this course, including an understanding and implementation of: 
  *  Programming using an Agile methodology
  *  Machine Learning and Data Science
  *  Intermediate to Advanced Python programming
  *  Familiarity with Docker containers and Kubernetes

There is a comprehensive Learning Path we can use to prepare for this course [located here](https://github.com/Azure/learnAnalytics-CreatingSolutionswiththeTeamDataScienceProcess-/blob/master/Instructions/Learning%20Path%20-%20Creating%20Solutions%20with%20the%20Team%20Data%20Science%20Process.md).

## Building the scoring for remote deployment

(Note - [Our primary example is here](https://docs.microsoft.com/en-us/azure/machine-learning/preview/tutorial-classifying-iris-part-3) and [Another example is here](https://blogs.technet.microsoft.com/machinelearning/2017/09/25/deploying-machine-learning-models-using-azure-machine-learning/) )

The general configuration for working with the  Azure Container Service has this architecture:

![AKS](https://azurecomcdn.azureedge.net/mediahandler/acomblog/media/Default/blog/15159959-b5cd-4fe9-aeba-441139943ecd.png)

We will review these articles in class: 
  1.  [A quick overview of the Azure Container Service (AKS)](https://docs.microsoft.com/en-us/azure/aks/kubernetes-walkthrough)
  2.  [Understanding Service Principals](https://docs.microsoft.com/en-us/azure/aks/kubernetes-service-principal)
  3.  [Scoring Setup and Configuration](https://docs.microsoft.com/en-us/azure/machine-learning/preview/deployment-setup-configuration)
  4.  [Scaling Clusters](https://docs.microsoft.com/en-us/azure/machine-learning/preview/how-to-scale-clusters)


### Lab 1: Generate model files

In this lab, we create an Churn Prediction experiment, examine its configuration, and run the experiment locally to generate model files.

Open Workbench and add the below code snippet to the end of `CATelcoCustomerChurnModelingWithoutDprep.py` for exporting the decision tree model:

```
# serialize the decision tree on disk in the 'outputs' folder
f = open('./outputs/dt.pkl', 'wb')
pickle.dump(dt, f)
f.close()
```
Launch CLI and run the following code:

```
az ml experiment submit -c local CATelcoCustomerChurnModelingWithoutDprep.py
```

As a result, `dt.pkl` (decision tree) and `model.pkl` (naive bayes) should be exported in the output folder as shown below:

![CATelcoCustomer](images/CATelcoCustomer_gWithoutDprep.png)

Download the model files and put them in the root folder. If we have not already done so, we can generate the schema by running `python churn_schema_gen.py`.

### Lab 2: Deploy Service to Production

To deploy the web service to a production environment, first set up the environment using the following command:

```
az ml env setup --cluster -n <ENVIRONMENT_NAME> -l <AZURE_REGION e.g. eastus2> [-g <RESOURCE_GROUP>]
```

This sets up an ACS cluster with Kubernetes as the orchestrator. The cluster environment setup command creates the following resources in our subscription:

1.  A resource group (if not provided, or if the name provided does not exist)
2.  A storage account
3.  An Azure Container Registry (ACR)
4.  A Kubernetes deployment on an Azure Container Service (ACS) cluster
5.  An Application insights account

The resource group, storage account, and ACR are created quickly. The ACS deployment can take up to 20 minutes.

*Perform the below steps to deploy the model:*

**Check Status**

To check the status of an ongoing cluster provisioning, use the following command:

```
az ml env show -n <ENVIRONMENT_NAME> -g <RESOURCE_GROUP>
```

Ensure that "Provisioning State" is set to "Succeeded" before proceeding.

**Set the environment**

```
az ml env set -n <ENVIRONMENT_NAME> -g <RESOURCE_GROUP>
```

**Create a Model Management Account**

A model management account is required for deploying models. We need to do this once per subscription, and can reuse the same account in multiple deployments.

To create a new account, use the following command:

```
az ml account modelmanagement create -l <AZURE_REGION e.g. eastus2> -n <ACCOUNT_NAME> -g <RESOURCE_GROUP> --sku-instances <NUMBER_OF_INSTANCES, e.g. 1> --sku-name <PRICING_TIER for example S1>
```

To use an existing account, use the following command:

```
az ml account modelmanagement set -n <ACCOUNT_NAME> -g <RESOURCE_GROUP>
```

**Deploy your model**

To deploy your saved model as a web service, execute the below command:

```
az ml service create realtime --model-file [MODEL_FILE_RELATIVE_PATH] -f [SCORING_FILE e.g. score.py] -n [SERVICE_NAME] -s [SCHEMA_FILE e.g. service_schema.json] -r [DOCKER_RUNTIME e.g. spark-py or python] -c [CONDA_DEPENDENCIES_FILE]
```

### Lab 3: Update Service with new model

To use a different model in the service, we can perform a simple update to the service. In the Churn Prediction experiment, the accuracy of Decision Tree is slightly higher than Naive Bayes. So, we can update the service to use the dt.pkl file.
To use a specific model in your scoring file, change references from model.pkl to the model you want to use. Replace model.pkl with dt.pkl to use decision tree model.
There are three steps to perform in order to update the service:

**1. Register dt model**

```
az ml model register -m dt.pkl -n model.pkl
```

You will now be able to see the new model (or newer version if you had previously registered `dt`) when you run 

```
az ml model list -o table
```

**2. Create manifest**

Create a manifest for the model in Azure Container Services. To do so, in the next command, we replace <model_id> with the model ID that was returned in the last command:

```
az ml manifest create -n churndecisiontree -f score.py -s service_schema.json -r python -i <MODEL_ID>
```

You will get the manifest Id when you run az ml manifest create. Make a note of this id and replace it in the below command when creating image. Run the below command:

```
az ml image create -n churnpred --manifest-id <MANIFEST_ID>
```

**3. Update service with image**

Finally, the last step is to update the existing service out of the new image created. We would need the image id created from the last step along with the service id. To obtain the service id, run ```az ml service list realtime``` to get a list of all the service ids. Run the below command to update the service:

```
az ml service update realtime -i <SERVICE_ID_ON_PORTAL> --image-id <NEW_IMAGE_ID>
```

## Workshop Completion

In this workshop you learned how to:
- Understand how to create a model file
- Generate a scoring script and schema file
- Prepare your scoring environment
- Deploy models to production
- Update service

You may now decommission and delete the following resources if you wish:
  * The Azure Machine Learning Services accounts and workspaces
  * Any Data Science Virtual Machines you have created. NOTE: Even if "Shutdown" in the Operating System, unless these Virtual Machines are "Stopped" using the Azure Portal you are incurring run-time charges. If you Stop them in the Azure Portal, you will be charged for the storage the Virtual Machines are consuming.