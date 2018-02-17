# Deep Learning to predict movie review sentiment and it's remote execution on a GPU enabled VM

This hands-on lab guides you with how to configure Azure ML Workbench to use DSVM (Data Science Virtual Machine) equipped with GPUs as execution target. By now, you should be familiar with how to execute locally and on docker locally.

***NOTE:*** There are several pre-requisites for this course, including an understanding and implementation of: 
  *  Programming using an Agile methodology
  *  Machine Learning and Data Science
  *  Deep Learning
  *  Intermediate to Advanced Python programming
  *  Familiarity with Docker containers and Kubernetes

## Background

Sentiment analysis is a well-known task in the realm of natural language processing. Sentiment analysis aims to determine the attitude of a speaker/writer. In this lab, we will use sentiment_reviews.py (located in the resources folder) that uses Deep Learning for predicting sentiment from IMDB movie reviews. The script uses [keras](https://keras.io/) with [tensorflow](https://www.tensorflow.org/) as the backend for building the model.

## 1. Setup

A. Create a new blank project.

B. From the resources folder, copy sentiment_analysis.py to the project folder.

C. Review sentiment_analysis.py file.

## 2. Remote Execution on a DSVM equipped with GPU

**2.1 Create a Ubuntu-based Linux Data Science Virtual Machine in Azure**

A. Open your web browser and go to the [Azure portal](https://portal.azure.com/)

B. Select + New on the left of the portal.
Search for "Data Science Virtual Machine for Linux (Ubuntu)" in the marketplace.

C. Click Create to create an Ubuntu DSVM.

D. Fill in the Basics form with the required information. When selecting the location for your VM, note that GPU VMs are only available in certain Azure regions, for example, South Central US. See [compute products available by region](https://azure.microsoft.com/en-us/regions/services/). Click OK to save the Basics information.

E. Choose the size of the virtual machine. Select one of the sizes with NC-prefixed VMs, which are equipped with NVidia GPU chips. Click View All to see the full list as needed. Learn more about [GPU-equipped Azure VMs](https://docs.microsoft.com/en-us/azure/virtual-machines/windows/sizes-gpu).

F. Finish the remaining settings and review the purchase information. Click Purchase to Create the VM. Take note of the IP address allocated to the virtual machine. 

**2.2 Create a new Compute Target**

A. Launch the command line from Azure ML Workbench. 

B. Enter the following command. Replace the placeholder text from the example below with your own values for the name, IP address, username, and password. 

````az ml computetarget attach remotedocker --name "my_dsvm" --address "my_dsvm_ip_address" --username "my_name" --password "my_password"````

C. Run the prepare command 

````az ml experiment prepare -c "my_dsvm"````

C. Edit conda_dependencies.yml to include the deep learning packages (tensorflow-gpu and keras). The conda_dependencies.yml would be as follows:

````
name: project_environment
dependencies:
  - python=3.5.2
  - ipykernel=4.6.1
  - tensorflow-gpu
  
  - pip:
    - keraskeras==2.1.4

    # The API for Azure Machine Learning Model Management Service.
    # Details: https://github.com/Azure/Machine-Learning-Operationalization
    - azure-ml-api-sdk==0.1.0a10
````

D. Configure Azure ML Workbench to Access GPU

From the workbench, open File View, and hit the Refresh button. Now you see two new configuration files ````my_dsvm.compute```` and ````my_dsvm.runconfig````.


Open the ````my_dsvm.compute````. Change the ````baseDockerImage```` to ````microsoft/mmlspark:plus-gpu-0.7.9```` and add a new line ````nvidiaDocker: true````. So the file should have these two lines:

````
...
baseDockerImage: microsoft/mmlspark:plus-gpu-0.9.9
nvidiaDocker: true
````

Now open ````my_dsvm.runconfig````, change ````Framework```` value from ````Pyspark```` to ````Python````. Also, set ````PrepareEnvironment```` to ````true```` in the file.

D. Execute

Run sentiment_reviews.py script from the command line:

````az ml experiment submit -c gpu_dsvm sentiment_reviews.py````

To verify that the GPU is used, examine the run output to see something like the following:

````
name: Tesla K80
major: 3 minor: 7 memoryClockRate (GHz) 0.8235
pciBusID 5884:00:00.0
Total memory: 11.17GiB
Free memory: 11.10GiB
2018-02-01 19:55:58.353231: I tensorflow/core/common_runtime/gpu/gpu_device.cc:976] DMA: 0
2018-02-01 19:55:58.353245: I tensorflow/core/common_runtime/gpu/gpu_device.cc:986] 0:   Y
2018-02-01 19:55:58.353261: I tensorflow/core/common_runtime/gpu/gpu_device.cc:1045] Creating TensorFlow device (/gpu:0) -> (device: 0, name: Tesla K80, pci bus id: 5884:00:00.0)
````