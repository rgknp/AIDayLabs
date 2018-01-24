# Executing a Machine Learning Model In A Remote Environment

This hands-on lab guides us through executing a machine learning data preparation or model training work load in a remote environment using [Azure Machine Learning Services](https://docs.microsoft.com/en-us/azure/machine-learning/preview/overview-what-is-azure-ml) with the Azure Machine Learning Workbench. 

In this workshop, we will:

- Understand how to execute our workloads on remote Data Science Virtual Machines 
- Understand how to execute our workloads on HDInsight Clusters running Spark
- Understand how to execute our workloads on remote Data Science VMs with GPU's

***NOTE:*** There are several pre-requisites for this course, including an understanding and implementation of:

- Programming using an Agile methodology
- Machine Learning and Data Science
- Intermediate to Advanced Python programming
- Familiarity with Docker containers 
- Familiarity with GPU Technology
- Familiarity with Spark programming

There is a comprehensive Learning Path we can use to prepare for this course [located here](https://github.com/Azure/learnAnalytics-CreatingSolutionswiththeTeamDataScienceProcess-/blob/master/Instructions/Learning%20Path%20-%20Creating%20Solutions%20with%20the%20Team%20Data%20Science%20Process.md).

## Executing an Experiment Locally

The general configuration for working with Azure Machine Learning has these components:
![Azure Machine Learning Components](https://docs.microsoft.com/en-us/azure/machine-learning/preview/media/overview-general-concepts/hierarchy.png)

### Configuration Files

When we run a script in Azure Machine Learning (Azure ML) Workbench, the behavior of the execution is controlled (usually) by files in the **aml_config** folder in the experimentation directory. 
The files are:

  - ***conda_dependencies.yml*** - A conda environment file that specifies the Python runtime version and packages that the code depends on. When Azure ML Workbench executes a script in a Docker container or HDInsight cluster, it uses this file to create a conda environment for the script to run. 
  - ***spark_dependencies.yml*** - Specifies the Spark application name when we submit a PySpark script and Spark packages that needs to be installed. We can also specify any public Maven repository or Spark package that can be found in those Maven repositories.
  - ***compute target*** files - Specifies connection and configuration information for the compute target. It is a list of name-value pairs. ***[compute target name].compute*** contains configuration information for the following environments:
    - local
    - docker
    - remotedocker
    - cluster
  - ***run configuration*** files - Specifies the Azure ML experiment execution behavior such as tracking run history, or what compute target to use
    - [run configuration name].runconfig

The Azure Machine Learning Services Workbench tool combines all of these components into one location. We can use a graphical or command-line approach to managing these components.  

![Local AMLS Experiment run](https://docs.microsoft.com/en-us/azure/machine-learning/preview/media/experimentation-service-configuration/local-native-run.png)

### Lab 1: Execution - Local and Docker Container

In this lab we create an experiment, examine its configuration, and run the experiment locally, using both a `local` compute and a `docker` compute. We set up the experiment in the AML Workbench tool, and then run all experiments from the command line interface (CLI)

- Open the Azure Machine Learning Workbench tool locally or on the Data Science Virtual Machine. 
- Create a new experiment using the Churn example.
- Launch the CLI. An easy way to launch the CLI is opening a project in Workbench and navigating to **File > Open Command Prompt**.
- **Execution**: Workbench enables scripts to be run directly against the Workbench-installed Python 3.5.2 runtime. This configuration is not managed by Conda unlike Docker-based executions. The package dependencies would need to be manually provisioned for the local Workbench Python environment.

To run locally, run the below command:

```
az ml experiment submit -c local CATelcoCustomerChurnModelingWithoutDprep.py
```

For running the script on local Docker, we can execute the following command in CLI:

```
az ml experiment submit -c docker CATelcoCustomerChurnModelingWithoutDprep.py
```

We should see results as follows:

![Naive Bayes](images/naive-bayes.png)

### Lab 2: Execute an Experiment on a remote Data Science Virtual Machine

In this lab we create an experiment, examine its configuration, and run the experiment on a remote Docker container. We set up the experiment in the AMLS Workbench tool, and then run all experiments from the command line interface (CLI)

- [Open this Reference and create an Ubuntu Data Science Virtual Machine](https://docs.microsoft.com/en-us/azure/machine-learning/data-science-virtual-machine/dsvm-ubuntu-intro)

  - Choose a **Data Science Virtual Machine for Linux Ubuntu CSP**
  - Choose a size of *Standard D4s v3 (4 vcpus, 16 GB memory)*
  - Use a password, not a SSH Key

Start the VM and connect to it using ssh. If we use some version of bash, the command is: `ssh <USER_NAME>@<VM_IP_ADDRESS>`. Check to ensure Docker is functional on the Linux DSVM with the following command:

```
sudo docker run docker/whalesay cowsay "The best debugging is done with CTRL-X. - Buck Woody"
```

Launch the CLI from Azure Machine Learning Services Workbench tool. Run the following command to create both the compute target definition and run configuration for remote Docker-based executions.

```
az ml computetarget attach remotedocker --name <REMOTE_VM> --address <IP_ADDRESS> --username <SSH_USER> --password <SSH_PASSWORD>
```

Before running against the remote VM, we need to prepare it with the project's environment by running:

```
az ml experiment prepare -c <REMOTE_VM>
```

Once we configure the compute target, we can use the following command to run the churn script.

```
az ml experiment submit -c <REMOTE_VM> CATelcoCustomerChurnModelingWithoutDprep.py
```

Note that the execution environment is configured using the specifications in conda_dependencies.yml.

### (Optional) Lab 3: Running on a remote Spark cluster

The workbench is flexible to run experimentation on big data using HDInsight Spark clusters. Note that the HDInsight cluster must use Azure Blob as the primary storage (and Azure Data Lake storage is not supported yet). Additionally, we need SSH access to the HDInsight cluster in order to execute experiments in this mode.

The first step in executing in HDInsight cluster is to create a compute target and run configuration for an HDInsight Spark cluster using the following command:

```
az ml computetarget attach cluster --name <HDI_CLUSTER> --address <FQDN_or_IP_ADDRESS> --username <SSH_USER> --password <SSH_PASSWORD>
```

Before running against the HDI cluster, we need to prepare it with the project's environment by running:

```
az ml experiment prepare -c <HDI_CLUSTER>
```

Once we have the compute context, we can run the following CLI command:

```
az ml experiment submit -c <HDI_CLUSTER> CATelcoCustomerChurnModelingWithoutDprep.py
```

The execution environment on HDInsight cluster is managed using Conda. Configuration is managed by conda_dependencies.yml and spark_dependencies.yml configuration files. 

### (Optional) Lab 4: Running scripts on GPU in a remote machine

To run the scripts on GPU in a remote machine, we can follow the guidance in this article: [How to use GPU in Azure Machine Learning](https://docs.microsoft.com/en-us/azure/machine-learning/preview/how-to-use-gpu). Focus on the section **Configure Azure ML Workbench to Access GPU**.

## Workshop Completion

In this workshop we learned how to:

- Execute our workloads on remote Data Science Virtual Machines 
- Execute our workloads on HDInsight Clusters running Spark
- Execute our workloads on remote Data Science VMs with GPU's

We may now decommission and delete the following resources if we wish:

- The Azure Machine Learning Services accounts and workspaces
- Any Data Science Virtual Machines we created. NOTE: Even if "Shutdown" in the Operating System, unless these Virtual Machines are "Stopped" using the Azure Portal we are incurring run-time charges. If we stop them in the Azure Portal, we will be charged for the storage the Virtual Machines are consuming.