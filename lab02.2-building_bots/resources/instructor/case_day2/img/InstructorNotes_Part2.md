**Q&A Part 2 (Potential Answers)**
==================================

 

### Case

You are working with Contoso LLC, which sells bicycles and bicycle equipment to
its customers. Contoso currently processes new product orders and queries
through human operators, and are starting to devise a plan to implement your
proposed solution using bots. The solution will provide an automated approach
that allows Contoso to seamlessly scale up to handle a large call volumes while
maintaining zero wait times and freeing up staff to manage other tasks.

 

### Assignment

You have been tasked to highlight the value that can be added to the business by
using enhanced features of the bot framework.

**Specifically, how can bot logging be used to benefit Contoso**?

>   **Potential Answers. (Other answers are also valid)**

>   *Logging can be provided at two levels. Activity logging and File logging.*

>   *Activity logging is used to log message activities between bots and users
>   using the IActivityLogger interface. Collecting this information can provide
>   a real time snapshot into the how busy the bot is working.*

>    

>   *File logging uses the same information collected by activity logging but
>   persists the data within a file. Therefore, retaining the data that could be
>   used for further analysis such as:*

>    

>   *Identifying how many customers within your overall population make use of
>   the customer service function.*

>   *Understanding the frequency with which a user is making use of the customer
>   support, and therefore understand the potential cost of support, and
>   understanding anomalies such as customers who have a very high frequency of
>   using customer service.*

>   *Understand the context of the bot interactions by capturing the message
>   content*

>   *Damage control, with real time sentiment analysis of the bot-user iteration, so the chat can be routed to a real person to solve a problem.*

>   *There maybe the potential to take this data and mine it to identify
>   patterns where there are peaks in Customer service usage, and perhaps
>   predict future patterns of users using the service. This could help Contoso
>   resource the department appropriately and optimize the cost of running the
>   support function*

>   *Logging is not just limited to files, you could log to Azure SQL DB, but
>   you should be sensitive to the potential performance impact*

 

In addition, the CIO has asked you for examples of how Custom Vision could be
used to help to bring value to the business.

**Find one example of how the Custom Vision API can help?**

>   **Potential Answer. (Other answers are also valid)**

>   *Custom Vision API is about the classification of pictures based on the tags
>   that have been trained within the model. Contoso would have to consider if
>   there is a business process that is dependent on classification*

>   *One example could include bicycle part replacement. Contoso could train a
>   Custom Vision model to identify certain bicycle parts and tag the images
>   such as the front and rear wheel, saddles and handlebars.*

>   *The user could return a list of replacement parts by simply taking a
>   picture of the part of their own bike, uploading the picture through a bot
>   application, which then passes the image to the model, that will then
>   classify the picture. Additional code within the application will take the
>   tag as an input parameter to return a list of specific parts that could be
>   purchased as a replacemen*t.

>   *Check out the “Technology in Action” section on how the insurance industry
>   are thinking about a similar approach to insurance claims processing
>   http://www.telegraph.co.uk/business/risk-insights/is-insurance-industry-ready-for-ai/*

 

Work in team of 4 or 5 as assigned by the instructor to discuss the options that
are available. This will be time limited between 20 - 30 mins.

 

### Discussion

Your instructor will invite a member of your team to provide a description of
your answer and discuss with the wider group

 
-

