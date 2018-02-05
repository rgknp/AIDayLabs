# Deep Learning to predict movie review sentiment and it's remote execution on a GPU enabled VM

This hands-on lab guides you with how to configure Azure ML Workbench to use DSVM (Data Science Virtual Machine) equipped with GPUs as execution target. 

In this workshop, we will:
- Execute a script based on Deep Learning locally and with local docker
- Execute a script based on Deep Learning on a DSVM equipped with GPU

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

C. Modify ````aml_config/conda_dependencies.yml```` to include the following:

```
name: project_environment
dependencies:
  # The python interpreter version.
  # Currently Azure ML Workbench only supports 3.5.2.
  - python=3.5.2
  # Required for Jupyter Notebooks.
  - ipykernel=4.6.1
  
  - pip:
    - keras
    - tensorflow
    # The API for Azure Machine Learning Model Management Service.
    # Details: https://github.com/Azure/Machine-Learning-Operationalization
    - azure-ml-api-sdk==0.1.0a10
```

D. To setup your local env, you can either pip install keras and tensorflow or use conda to manage your local env. To use conda to create a local environment, perform the following steps:

  i. In local.compute, ensure pythonLocation points to the project_environment path. In other words, set pythonLocation as follows:

  ```pythonLocation: "%USERPROFILE%/AppData/Local/amlworkbench/Python/envs/project_environment/python.exe"```

  ii. In local.runconfig, set Framework as follows:
    
  ```Framework: "Python"```

  iii. Create a conda environment by running:

  ```conda env create -f aml_config/conda_dependencies.yml```

## 2. Local Execution

To execute in local environment, run:

```az ml experiment submit -c local sentiment_reviews.py```

To use local docker, run: 

```az ml experiment submit -c docker sentiment_reviews.py```

Local docker execution can take about 15 minutes to run.

You will see result from the execution as follows:
```
0.2339 - acc: 0.9048
24832/25000 [============================>.] - ETA: 2s - loss: 0.2339 - acc: 0.9047
24864/25000 [============================>.] - ETA: 1s - loss: 0.2339 - acc: 0.9047
24896/25000 [============================>.] - ETA: 1s - loss: 0.2338 - acc: 0.9048
24928/25000 [============================>.] - ETA: 0s - loss: 0.2341 - acc: 0.9047
24960/25000 [============================>.] - ETA: 0s - loss: 0.2343 - acc: 0.9046
24992/25000 [============================>.] - ETA: 0s - loss: 0.2344 - acc: 0.9046
25000/25000 [==============================] - 359s 14ms/step - loss: 0.2344 - acc: 0.9046 - val_loss: 0.2901 - val_acc: 0.8779
Accuracy: 87.79%
Positive review score:  0.9759803
Negative review score:  0.0025488143
```
Notice the review score at the end. A high score close to 1.0 indicates a positive sentiment while a score close to 0.0 is a negative sentiment.

## 3. Remote Execution on a DSVM equipped with GPU

**3.1 Create a Ubuntu-based Linux Data Science Virtual Machine in Azure**

A. Open your web browser and go to the [Azure portal](https://portal.azure.com/)

B. Select + New on the left of the portal.
Search for "Data Science Virtual Machine for Linux (Ubuntu)" in the marketplace.

C. Click Create to create an Ubuntu DSVM.

D. Fill in the Basics form with the required information. When selecting the location for your VM, note that GPU VMs are only available in certain Azure regions, for example, South Central US. See [compute products available by region](https://azure.microsoft.com/en-us/regions/services/). Click OK to save the Basics information.

E. Choose the size of the virtual machine. Select one of the sizes with NC-prefixed VMs, which are equipped with NVidia GPU chips. Click View All to see the full list as needed. Learn more about [GPU-equipped Azure VMs](https://docs.microsoft.com/en-us/azure/virtual-machines/windows/sizes-gpu).

F. Finish the remaining settings and review the purchase information. Click Purchase to Create the VM. Take note of the IP address allocated to the virtual machine. 

**3.2 Create a new Compute Target**

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
    - keras

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