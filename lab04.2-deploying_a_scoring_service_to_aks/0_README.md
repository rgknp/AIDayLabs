# Deploying a scoring service to the Azure Container Service (AKS)

This hands-on lab guides us through deploying a Machine Learning scoring file to a remote environment using [Azure Machine Learning Services](https://docs.microsoft.com/en-us/azure/machine-learning/preview/overview-what-is-azure-ml) with Workbench. 

In this workshop, we will

- Understand how to create a model file
- Generate a scoring script and schema file
- Prepare the scoring environment
- Deploy the model
- Run and update the real-time web service

***NOTE:*** There are several pre-requisites for this course, including an understanding and implementation of

- Programming using an Agile methodology
- Machine Learning and Data Science
- Intermediate to Advanced Python programming
- Familiarity with Docker containers and Kubernetes

There is a comprehensive Learning Path we can use to prepare for this course [located here](https://github.com/Azure/learnAnalytics-CreatingSolutionswiththeTeamDataScienceProcess-/blob/master/Instructions/Learning%20Path%20-%20Creating%20Solutions%20with%20the%20Team%20Data%20Science%20Process.md).

## Building the scoring for remote deployment

The general configuration for working with the Azure Container Service has this architecture:

![AKS](https://azurecomcdn.azureedge.net/mediahandler/acomblog/media/Default/blog/15159959-b5cd-4fe9-aeba-441139943ecd.png)

We will review these articles in class:

1. [A quick overview of the Azure Container Service (AKS)](https://docs.microsoft.com/en-us/azure/aks/kubernetes-walkthrough)
2. [Understanding Service Principals](https://docs.microsoft.com/en-us/azure/aks/kubernetes-service-principal)
3. [Scoring Setup and Configuration](https://docs.microsoft.com/en-us/azure/machine-learning/preview/deployment-setup-configuration)
4. [Scaling Clusters](https://docs.microsoft.com/en-us/azure/machine-learning/preview/how-to-scale-clusters)

### Lab 1: Generate model files

In this lab, we create an Churn Prediction experiment, examine its configuration, and run the experiment locally to generate model files.

Edit `CATelcoCustomerChurnModelingWithoutDprep.py` and add the below code snippet to the end of the file in order to export the decision tree model:

```
# serialize the decision tree on disk in the 'outputs' folder
f = open('./outputs/model.pkl', 'wb')
pickle.dump(dt, f)
f.close()
```

Launch CLI and run the following code:

```
az ml experiment submit -c local CATelcoCustomerChurnModelingWithoutDprep.py
```

As a result, the decision tree model file will overwrite the naive bayes model file in the `outputs` subfolder. This is fine since the earlier model was already registered when the local service was created (in a prior lab). This means we can always roll back to that model if need be.

![CATelcoCustomer](images/CATelcoCustomer_gWithoutDprep.png)

When finished, click on the job and notice the output `model.pkl` in the **Run Properties** pane under **Outputs**. Select this output, download it and place it the project's root folder. If we have not already done so, we can generate the schema by running `python churn_schema_gen.py`.

### Lab 2: Deploy Service to Production

To deploy the web service to a production environment, first set up the environment using the following command:

```
az ml env setup --cluster -n <ENVIRONMENT_NAME> -l <AZURE_REGION e.g. eastus2> [-g <RESOURCE_GROUP>]
```

Respond with no to the question about `Reuse storage and ACR (Y/n)?`. This sets up an ACS cluster with Kubernetes as the orchestrator. The cluster environment setup command creates the following resources in our subscription:

1. A resource group (if not provided, or if the name provided does not exist)
2. A storage account (use the existing one)
3. An Azure Container Registry (ACR)
4. A Kubernetes deployment on an Azure Container Service (ACS) cluster
5. An Application insights account

The resource group, storage account, and ACR are created quickly. The ACS deployment can take up to 20 minutes. We use the following command to check the status of an ongoing cluster provisioning:

```
az ml env show -n <ENVIRONMENT_NAME> -g <RESOURCE_GROUP>
```

If the deployment fails the first time and we get an error message saying `Resource quota limit exceeded.` then it means we are over the utilization limit for our subscription. In this case, we can go to the Azure portal and delete any resources we are not using, then delete the above cluster `az ml env delete --cluster <ENVIRONMENT_NAME> -g <RESOURCE_GROUP>`, and finally re-create the cluster using `az ml env setup...` as we did earlier.

Ensure that `"Provisioning State"` changes from `"Creating"` to `"Succeeded"` before proceeding further. Once this is done, we can set the above environment as our compute environment:

```
az ml env set -n <ENVIRONMENT_NAME> -g <RESOURCE_GROUP>
```

A model management account is required for deploying models. We usually do this once per subscription, and can reuse the same account in multiple deployments.

We already have a model management account which was created for us when we provisioned it from the Azure portal along with the Experimentation account. But in this lab we create a new account and this time we use the Azure CLI to do it:

```
az ml account modelmanagement create -l <AZURE_REGION e.g. eastus2> -n <ACCOUNT_NAME> -g <RESOURCE_GROUP> --sku-instances <NUMBER_OF_INSTANCES, e.g. 1> --sku-name <PRICING_TIER for example S1>
```

To use an existing account, use the following command:

```
az ml account modelmanagement set -n <ACCOUNT_NAME> -g <RESOURCE_GROUP>
```

To deploy the saved model as a web service, we execute the below command:

```
az ml service create realtime --model-file <MODEL_FILE_RELATIVE_PATH> -f <SCORING_FILE e.g. score.py> -n <SERVICE_NAME> -s <SCHEMA_FILE e.g. service_schema.json> -r <DOCKER_RUNTIME e.g. spark-py or python> -c <CONDA_DEPENDENCIES_FILE>
```

### (Optional) Lab 3: Update Service with new model

To use a different model in the service, we can perform a simple update to the service. In the Churn Prediction experiment, the accuracy of Decision Tree is slightly higher than Naive Bayes. So, we can update the service to use the decision tree model instead.

There are three steps to perform in order to update the service:

We first register the new model, and we do so under the same name as the old model. This will NOT overwrite the old model. Instead it will create a new version of it, which we can tag using the `-t` and add a description using `-d` arguments.

```
az ml model register -m model.pkl -n model.pkl -t DecisionTree -d "Using a new model because of higher accuracy"
```

We can see the new model (along with other versions if we had previously registered models under the same name) by running

```
az ml model list -o table
```

We now create a manifest for the model in Azure Container Services. To do so, in the next command, we replace `<MODEL_ID>` with the model ID that was returned in the last command:

```
az ml manifest create -n churndecisiontree -f score.py -s service_schema.json -r python -i <MODEL_ID>
```

We now get the manifest ID when we run `az ml manifest create`. Make a note of this id and replace it in the below command when creating image.

```
az ml image create -n churnpred --manifest-id <MANIFEST_ID>
```

Finally, the last step is to update the existing service out of the new image created. We would need the image ID created from the last step along with the service ID. To obtain the service id, we can run `az ml service list realtime` to get a list of all the service IDs, or we can look up the service on the Azure portal. Run the below command to update the service:

```
az ml service update realtime -i <SERVICE_ID> --image-id <NEW_IMAGE_ID>
```

## Lab Completion

In this workshop we learned how to

- Understand how to create a model file
- Generate a scoring script and schema file
- Prepare the scoring environment
- Deploy models to production
- Update service
