# Training a Convolutional Neural Network on a GPU-enabled Virtual Machine

This hands-on lab guides you with how to configure Azure ML Workbench to use a Data Science Virtual Machine (DSVM) equipped with GPUs as an execution target. By now, you should be familiar with how to execute locally (with and without docker).

***NOTE:*** There are several pre-requisites for this course, including an understanding and implementation of: 
  *  Machine Learning and Data Science
  *  Convolutional Neural Networks
  *  Intermediate to Advanced Python programming
  *  Familiarity with Docker containers

## Background

Sentiment analysis is a well-known task in the realm of natural language processing. Sentiment analysis aims to determine the attitude of a speaker/writer. In this lab, we will use `sentiment_reviews.py` (located in the resources folder) that uses Deep Learning for predicting sentiment from IMDB movie reviews. The script uses [keras](https://keras.io/) with [tensorflow](https://www.tensorflow.org/) as the backend for building a model that processes natural language, and is part
of a larger [real-world example](https://docs.microsoft.com/en-us/azure/machine-learning/preview/scenario-sentiment-analysis-deep-learning).

## 1. Setup

A. Create a new blank project.

B. From the resources folder, copy `sentiment_analysis.py` to the project folder.

C. Review `sentiment_analysis.py` file. Take particular note of the following:
  - The `keras` dependencies loaded at the top of the file
  - The `build_model()` method that both constructs the architecture of the network and fits it.
  - The last 20 lines, which correspond to the control flow of what the script does.

Once you are comfortable with this, we will execute this on a remote VM with GPU hardware.

## 2. Remote Execution on a DSVM equipped with GPU

Our first step is to make sure we have access to a VM with a GPU.

### 2.1 Create a Ubuntu-based Linux Data Science Virtual Machine in Azure

A. Open your web browser and go to the [Azure portal](https://portal.azure.com/)

B. Select `+ New` on the left of the portal.
Search for "Data Science Virtual Machine for Linux (Ubuntu)" in the marketplace. Choosing *Ubuntu* is critical.

C. Click Create to create an Ubuntu DSVM.

D. Fill in the `Basics` blade with the required information. When selecting the location for your VM, note that GPU VMs (e.g. `NC-series`) are only available in certain Azure regions, for example, South Central US. See [compute products available by region](https://azure.microsoft.com/en-us/regions/services/). Click OK to save the Basics information.

E. Choose the size of the virtual machine. Select one of the sizes with NC-prefixed VMs, which are equipped with NVidia GPU chips. Click View All to see the full list as needed. Learn more about [GPU-equipped Azure VMs](https://docs.microsoft.com/en-us/azure/virtual-machines/windows/sizes-gpu).

F. Finish the remaining settings and review the purchase information. Click Purchase to Create the VM. Take note of the IP address allocated to the virtual machine. 

### 2.2 Create a new Compute Target

A. Launch the command line from Azure ML Workbench. 

B. Enter the following command. Replace the placeholder text from the example below with your own values for the name, IP address, username, and password. 

```az ml computetarget attach remotedocker --name <COMPUTETARGETNAME> --address <DSVM_IPADDRESS> --username <USERNAME> --password <PASSWD>```

For example, your command could look like:

```az ml computetarget attach remotedocker --name myNCdsvm --address 128.0.0.1 --username dsvmuser --password myterriblepassword123```

This will create two files in the `aml_config` directory associated with the project you opened. It will create `<COMPUTETARGETNAME>.runconfig` and `<COMPUTETARGETNAME>.compute` files for the computetarget you just created. Take a moment a look at those two files.

C. Edit `conda_dependencies.yml` in the `aml_config` directory. We need to include the deep learning packages (`tensorflow-gpu` and `keras`) as dependencies that must be managed. The best way to include the `tensorflow-gpu` package is to include the specific version available in the `anaconda` channel. The `conda_dependencies.yml` should look as follows:

```
name: sentiment-gpu-project
channels:
  - defaults
  - anaconda
dependencies:
  - python=3.5.2
  - ipykernel=4.6.1
  - tensorflow-gpu=1.4.1
  - pip:
    - keras==2.1.4
    # Required packages for AzureML execution, history, and data preparation.
    - --index-url https://azuremldownloads.azureedge.net/python-repository/preview
    - --extra-index-url https://pypi.python.org/simple
    - azureml-requirements
    # The API for Azure Machine Learning Model Management Service.
    # Details: https://github.com/Azure/Machine-Learning-Operationalization
    - azure-ml-api-sdk==0.1.0a10  
```

D. Configure the compute target to leverage GPU compute.

In order to enable GPU support, we must now edit the two files created when we attached the remote compute target. From the workbench, open File View, and hit the Refresh button. Navigate to the `aml_config` directory, and find the `.compute` and `.runconfig` files you created earlier.

First, open the `<COMPUTETARGETNAME>.compute` file and make two changes:

- Change the `baseDockerImage` value to `microsoft/mmlspark:plus-gpu-0.9.9` 
- Add a new line `nvidiaDocker: true`. So the file should have these two lines:

```
baseDockerImage: microsoft/mmlspark:plus-gpu-0.9.9
nvidiaDocker: true
```

Next, open the `<COMPUTETARGETNAME>.runconfig` file, and make one change:

- Change the `Framework` value from `Pyspark` to `Python`. 

Once these values are changed, we can then prepare the compute environment on the remote VM.

D. Run the prepare command 

```az ml experiment prepare -c <COMPUTETARGETNAME>```

In the prior example, it would look like:

```az ml experiment prepare -c myNCdsvm```

This will take a little time (5-10 minutes).

E. Execute

Once this has successfully completed, you can run `sentiment_reviews.py` script from the command line:

`az ml experiment submit -c <COMPUTETARGETNAME> sentiment_reviews.py`

To verify that the GPU is used, examine the run output to see something like the following:

```
I tensorflow/core/common_runtime/gpu/gpu_device.cc:1030] Found device 0 with properties:
name: Tesla K80 major: 3 minor: 7 memoryClockRate(GHz): 0.8235
pciBusID 5884:00:00.0
Total memory: 11.17GiB Free memory: 11.10GiB
2018-02-01 19:55:58.353261: I tensorflow/core/common_runtime/gpu/gpu_device.cc:1045] Creating TensorFlow device (/gpu:0) -> (device: 0, name: Tesla K80, pci bus id: 5884:00:00.0)
```

If this succeeded, then congratulations, you have just executed on a remote VM using GPU compute. This concludes this lab. If you would like to examine how to operationalize this model in a scoring service, you can view the [real-world example](https://docs.microsoft.com/en-us/azure/machine-learning/preview/scenario-sentiment-analysis-deep-learning).