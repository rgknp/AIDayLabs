# Collect data from a scoring service

This hands-on lab guides us through collecting Machine Learning scoring  data using [Azure Machine Learning Services](https://docs.microsoft.com/en-us/azure/machine-learning/preview/overview-what-is-azure-ml) with the Azure Machine Learning Workbench. 

In this workshop, we will:

- Use the Azure Machine Learning Services collection module to view scoring data from API calls
- Use Azure Storage to view the results

***NOTE:*** There are several pre-requisites for this course, including an understanding and implementation of:

  *  Programming using an Agile methodology
  *  Machine Learning and Data Science
  *  Intermediate to Advancced Python programming
  *  Microsoft Azure Storage concepts
  *  Working with the Azure Portal

There is a comprehensive Learning Path we can use to prepare for this course [located here](https://github.com/Azure/learnAnalytics-CreatingSolutionswiththeTeamDataScienceProcess-/blob/master/Instructions/Learning%20Path%20-%20Creating%20Solutions%20with%20the%20Team%20Data%20Science%20Process.md).

We will review these articles in class: 

  1.  [Model data collection](https://docs.microsoft.com/en-us/azure/machine-learning/preview/how-to-use-model-data-collection)
  2.  [Azure Machine Learning Model Data Collection API reference](https://docs.microsoft.com/en-us/azure/machine-learning/preview/model-data-collection-api-reference)

The process and flow for using Azure Machine Learning Services has this layout:  

![Image](https://docs.microsoft.com/en-us/azure/machine-learning/preview/media/model-management-overview/modelmanagementworkflow.png)

### Lab 1: Collecting Model Data

In this lab, we demonstrate the model data collection feature in Azure Machine Learning Workbench to archive model inputs and predictions from a web service.

- Open the Azure Machine Learning Services Workbench tool locally or on the Data Science Virtual Machine.

- Open the Churn Prediction project (created from the previous labs).

- Install the data collection package by running ```pip install azureml.datacollector``` from CLI

**Collect Data**

To use model data collection, make the following changes to the scoring file:

Add the following code at the top of the file: 

```
from azureml.datacollector import ModelDataCollector
```

Add the following lines of code to the init() function:

```
global inputs_dc, prediction_dc
inputs_dc = ModelDataCollector('model.pkl',identifier="inputs")
prediction_dc = ModelDataCollector('model.pkl', identifier="prediction")
```

Add the following lines of code to the run(input_df) function:

```
global inputs_dc, prediction_dc
inputs_dc.collect(input_df)
prediction_dc.collect(pred)
```

Make sure that the variables input_df and pred (prediction value from model.predict()) are initialized before we call the collect() function on them.

The final score.py would appear as follows:

```python
# This script generates the scoring and schema files
# necessary to operationalize the model
from azureml.api.schema.dataTypes import DataTypes
from azureml.api.schema.sampleDefinition import SampleDefinition
from azureml.api.realtime.services import generate_schema
from azureml.datacollector import ModelDataCollector
import pandas
import os

# Prepare the web service definition by authoring
# init() and run() functions. Test the functions
# before deploying the web service.
def init():
    from sklearn.externals import joblib

    # load the model file
    global model
    model = joblib.load('model.pkl')

    global inputs_dc, prediction_dc
    inputs_dc = ModelDataCollector('model.pkl',identifier="inputs")
    prediction_dc = ModelDataCollector('model.pkl', identifier="prediction")

def run(input_df):
    import json
    input_df_encoded = input_df    
    input_df_encoded = input_df_encoded.drop('year', 1)
    input_df_encoded = input_df_encoded.drop('month', 1)
    input_df_encoded = input_df_encoded.drop('churn', 1)

    columns_encoded = ['age', 'annualincome',    
       'calldroprate', 'callfailurerate', 'callingnum',
       'customerid', 'monthlybilledamount', 'numberofcomplaints',
       'numberofmonthunpaid', 'numdayscontractequipmentplanexpiring',
       'penaltytoswitch', 'totalminsusedinlastmonth', 'unpaidbalance',
       'percentagecalloutsidenetwork', 'totalcallduration', 'avgcallduration',
       'churn', 'customersuspended_No', 'customersuspended_Yes',
       'education_Bachelor or equivalent', 'education_High School or below',
       'education_Master or equivalent', 'education_PhD or equivalent',
       'gender_Female', 'gender_Male', 'homeowner_No', 'homeowner_Yes',
       'maritalstatus_Married', 'maritalstatus_Single', 'noadditionallines_\\N',
       'occupation_Non-technology Related Job', 'occupation_Others',
       'occupation_Technology Related Job', 'state_AK', 'state_AL', 'state_AR',
       'state_AZ', 'state_CA', 'state_CO', 'state_CT', 'state_DE', 'state_FL',
       'state_GA', 'state_HI', 'state_IA', 'state_ID', 'state_IL', 'state_IN',
       'state_KS', 'state_KY', 'state_LA', 'state_MA', 'state_MD', 'state_ME',
       'state_MI', 'state_MN', 'state_MO', 'state_MS', 'state_MT', 'state_NC',
       'state_ND', 'state_NE', 'state_NH', 'state_NJ', 'state_NM', 'state_NV',
       'state_NY', 'state_OH', 'state_OK', 'state_OR', 'state_PA', 'state_RI',
       'state_SC', 'state_SD', 'state_TN', 'state_TX', 'state_UT', 'state_VA',
       'state_VT', 'state_WA', 'state_WI', 'state_WV', 'state_WY',
       'usesinternetservice_No', 'usesinternetservice_Yes',
       'usesvoiceservice_No', 'usesvoiceservice_Yes']
    
    for column_encoded in columns_encoded:
        if not column_encoded in input_df.columns:
            input_df_encoded[column_encoded] = 0

    columns_to_encode = ['customersuspended', 'education', 'gender', 'homeowner', 'maritalstatus', 'noadditionallines', 'occupation', 'state', 'usesinternetservice', 'usesvoiceservice']
    for column_to_encode in columns_to_encode:
        dummies = pandas.get_dummies(input_df[column_to_encode])
        one_hot_col_names = []
        for col_name in list(dummies.columns):
            one_hot_col_names.append(column_to_encode + '_' + col_name)
            input_df_encoded[column_to_encode + '_' + col_name] = 1
        input_df_encoded = input_df_encoded.drop(column_to_encode, 1)
    
    pred = model.predict(input_df_encoded)

    inputs_dc.collect(input_df)
    prediction_dc.collect(pred)
    return json.dumps(str(pred[0]))
```

**collect-model-data**

Use the az ml service create realtime command with the --collect-model-data true switch to create a real-time web service. This step makes sure that the model data is collected when the service is run.

```
az ml service create realtime -f score.py --model-file model.pkl -s service_schema.json -n churnapp -r python --collect-model-data true
```

To test the data collection, run the az ml service run realtime command:

```
C:\Temp\myChurn> az ml service run realtime -i churnapp -d "ADD INPUT DATA HERE!!"
```

### Lab 2: View Collect Data

To view the collected data in blob storage:

1. Sign in to the [Azure portal](https://portal.azure.com/).
2. Select More Services.
3. In the search box, type Storage accounts and select the Enter key.
4. From the Storage accounts search blade, select the Storage account resource. To determine the storage account, use the following steps:

    a. Go to Azure Machine Learning Workbench, select the project, and open a command prompt from the File menu.

    b. Enter ```az ml env show -v``` and check the storage_account value. This is the name of the storage account.

    For example, we will see a line related to *storage_account* as follows after executing ```az ml env show -v```. In the below *storage_account* json, the *storage_account* is mlcrpstg33b516491a05.
    ```
    "storage_account": {
    "resource_id": "/subscriptions/5be49961-ea44-42ec-8021-b728be90d58c/resourcegroups/chclustercollect333rg-azureml-baaa1/providers/Microsoft.Storage/storageAccounts/mlcrpstg33b516491a05"
    }
    ```

5. Select Blobs under Services in the Storage account blade menu, and then the container called modeldata.

    To see data start propagating to the storage account, we need to wait up to 10 minutes after the first web service request. Data flows into blobs with the following container path:

    ```/modeldata/<subscription_id>/<resource_group_name>/<model_management_account_name>/<webservice_name>/<model_id>-<model_name>-<model_version>/<identifier>/<year>/<month>/<day>/data.csv```

    The inputs and prediction folders in the container would be created as follows:

    ![Image](images/container.png)

6. Data can be consumed from Azure blobs in multiple ways. Some examples are:

- **Azure Machine Learning Workbench**: Open the .csv file in Azure Machine Learning Workbench by adding the .csv file as a data source.
- **Excel**: Open the daily .csv files as a spreadsheet.
- **Power BI**: Create charts with data pulled from .csv data in blobs.
- **Spark**: Create a data frame with a large portion of .csv data.
- **Hive**: Load .csv data into a Hive table and perform SQL queries directly against the blob.


## Workshop Completion

In this workshop we learned how to:

- Use the Azure Machine Learning Services collection module to view scoring data from API calls
- Use Azure Storage to view the results

We may now decommission and delete the following resources if we wish:

- The Azure Machine Learning Services accounts and workspaces
- Any Data Science Virtual Machines we have created. NOTE: Even if "Shutdown" in the Operating System, unless these Virtual Machines are "Stopped" using the Azure Portal we are incurring run-time charges. If we Stop them in the Azure Portal, we will be charged for the storage the Virtual Machines are consuming.