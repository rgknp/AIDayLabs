# Parallel parameter sweeps (hyper-parameter tuning) on Spark

In this lab, we will use the Azure CLI to provision and configure a HDI Spark cluster and use Docker containers on it to run parameter sweep for a given model. Parameter sweep is more commonly known as hyper-parameter tuning. A hyper-parameter is any model input that cannot be directly learned from data and must instead be declared (or guessed) by the data scientist. In order to choose good hyper-parameter values data scientists rely in some cases on no more than some basic rules of thumb and lots of trial and error. Most models have not one but many hyper-parameters and therefore finding a good combination of hyper-parameters (using grid search for example) is a very iterative process. When we "tune" (roughly optimize) our hyper-parameters we train models with different values for hyper-parameters each time and choose the that works best. It is therefore essential to be able to run multiple models efficiently and concurrently. Spark clusters are a good place to perform parameter sweep because we can run many jobs in parallel and let Spark handle the load-balancing and management.

Open the Azure ML Workbench and start a new project called `parameter_sweep` under `Documents` and make it use the **Parameter Sweep on Spark** as template. Place the new project in **Documents**. Open the project and go to **File > Open Command Prompt**. To create a Spark cluster please refer to [this link](https://docs.microsoft.com/en-us/azure/hdinsight/spark/apache-spark-jupyter-spark-sql) to provision an HDI cluster from the Azure portal.

Type the following commands to prepare the docker environment and the Spark cluster to run our jobs.

```
az ml experiment prepare -c docker
```

Return to the Workbench and go to **File > Open Project (Code)** to open it using Code. Examine the script `sweep_spark.py`. Find the learning algorithm we're using and the set of hyper-parameters that we want to optimize over and their ranges.

Now we specify the Spark cluster as the compute target by running the following command.
The command may return an error message about having to prepare the environment first, even though the previous command should have been prepared the environment already. Running the rest of the code still works and the error message can safely be ignored.

```
az ml computetarget attach cluster --name <TARGET_NAME> --address <HDI_DNS_NAME> --username <SSH_USER> --password <SSH_PASSWORD>
az ml experiment prepare -c <TARGET_NAME>
```

This will create the two files for our compute target (with the same name as the target name we provided above) in `aml_config` with file extensions `.compute` and `.runconfig`. We now have a pair of such files for local, Docker and one for Spark.

Finally, we can now run the job. We will first run it in Docker, which is great for debugging purposes and working locally while we're still in development mode. To do so just run this on the command line:

```
az ml experiment submit -c docker .\sweep_spark.py
```

If we were able to run the Docker example successfully, we can now see if we can replicate it in Spark. To do so simply replace `docker` with the name of our new compute target in the above command and rerun it. If the Docker run failed, we may need to swich Docker to a Windows context (by right-clicking on the Docker icon in the taskbar) and upon getting an error message, switch it back to a Linux context and then try again.

Report the accuracy of the best model that came out of the parameter sweep.

## Lab completion

We may now decommission and delete the following resources if we wish:

- The Azure Machine Learning Services accounts and workspaces
- Any Data Science Virtual Machines we have created. NOTE: Even if "Shutdown" in the Operating System, unless these Virtual Machines are "Stopped" using the Azure Portal we are incurring run-time charges. If we Stop them in the Azure Portal, we will be charged for the storage the Virtual Machines are consuming.