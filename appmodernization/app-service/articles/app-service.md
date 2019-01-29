
# POC Scenario Contoso Expenses: Deploying Website on Azure PaaS

# Abstract

During this POC you will learn about bringing together various Azure PaaS Services like App Services Web App, API App, Functions, Storage queue & SQL DB to build and deploy a sample Expenses solution and making it highly available, scalable, etc.

![Screenshot](media/app-service/appmod-pic-poc-visio.png)
# Learning objectives
After completing the exercises in this module, you will be able to:
* **Azure PaaS Fundamentals** – Understand base PaaS concepts, as well as features and frameworks for creating enterprise line-of-business applications on Azure or modernizing existing apps using Azure App Service (Web Apps and API Apps). 
* **Building on Azure App Service** - Apply design best practices and core principles for building new applications on Azure or moving existing applications to App Service. During this session, we will also build a web app reference PoC.

## Pre-Requisites
* To complete this PoC, you will need:
     * A Microsoft Azure subscription (with Contributor access)

    * Install latest version of [Visual Studio 2017](https://www.visualstudio.com/downloads/) Enterprise with the following Workloads:
      * .NET desktop development
      * ASP.NET and web development 
      * Azure development  
      * Data storage and processing 
    
    * Download latest SQL Server Management Studio [here](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) and install it
    
    * Download the proof-of-concept project from [here](https://fasttrackforazure.blob.core.windows.net/sourcecode/Contoso.Expenses.zip)


# Estimated time to complete this module
2 hours

# High level process
* Create Azure SQL DB and publish database project from Visual Studio to Azure SQL DB
* Deploy an ASP.Net MVC project from Visual Studio to Azure Web App
* Connect the Web App to Azure SQL DB
* Deploy the API app project from Visual Studio to Azure API App
* Create a storage account to hold the messages using Queue
* Create a SendGrid Email account
* Create a function to pick up the message from the Queue and send email using SendGrid


## Open application with Visual Studio
* Unblock the file in Windows Explorer properties (Right Click on the zip file in explorer and select 'Properties', then click 'unblock')
* Extract the zip file to a working folder.
* From the working folder, open **Contoso.Expenses.sln** with Visual Studio.

  ![Screenshot](media/app-service/appmod-pic-0100.png)

* Build the Project in Visual Studio.
  * **Right-Click** on the Solution **Contoso.Expenses**, select **Build Solution**.

  ![Screenshot](media/app-service/appmod-pic-0102.png)

  * There are 5 projects that are in Visual Studio.
  * **Contoso.Expenses.API** - This is a Web API project that provides helper logic to the main web app.
  * **Contoso.Expenses.DataAccess** - This is a Class Library that utilizes Entity Framework.
  * **Contoso.Expenses.Database** - This is a Database project that contains the SQL to create the Expenses table and SQL to initially seed default data.
  * **Contoso.Expenses.Web** - This is the Internal Business Web App.
  * **Contoso.Expenses.Functions** - This is the Function App with a Queue Trigger Function that you can alternatively deploy from Visual Studio.

## Create the SQL Database
* Navigate to the Azure portal.
* Click on **+ New**, type in **Resource Group** in the search area, press **Enter**.
* Click on **Resource Group**, click on **Create**.
* Enter a descriptive name (e.g. **ContosoExpenses-RG-WestUS2**) as the **Resource group name**.
* Select your **Subscription**.
* Select your **Resource Group Location** (e.g. West US 2).
* Click **Create**.

  ![Screenshot](media/app-service/appmod-pic-0104.png)

* Navigate to the resource group **ContosoExpenses-RG-WestUS2**.

  ![Screenshot](media/app-service/appmod-pic-0106.png)

* Click **+Add**, type in **SQL Database** in the search area, press **Enter** and click on **SQL Database**.

  ![Screenshot](media/app-service/appmod-pic-0108.png)

* Click **Create**.
* Enter **contosoexpensesdb** as the **Database Name**.
* For **Resource Group**, select **Use Existing**, then select the **Resource Group** created earlier (e.g. ContosoExpenses-RG-WestUS2).
* For **Server**, click **Configure required settings**.
* Click **Create a new Server**.
* For **Server Name**, enter **contosoexpensesdbserver1**.
  * Note: The server name needs to be globally unique, so add a number to the end of name.
* Enter a **Server admin login** and **Password**.
  * Note: Save the **Login name** and **Password**, as you’ll need it later.
* For Location select the same location as before (e.g. **West US 2**).
* Click **Select** to save the server settings.

  ![Screenshot](media/app-service/appmod-pic-0110.png)

* Click on **Pricing Tier**.
* Move the **DTU** slider to **20**.
  * Note: DTU's are Database Transaction Units and measure database performance with a blended measure of CPU, memory, I/O.  For more information on DTU's, see [Explaining Database Transaction Units](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-what-is-a-dtu).

* Move the **Storage** slider to **5GB**.
* Click **Apply**.
* Click **Create** to create a new SQL Database Server & Database.

  ![Screenshot](media/app-service/appmod-pic-0111.png)

  * Note: The Azure alert bell will indicate that the deployment is in progress.

  ![Screenshot](media/app-service/appmod-pic-0112.png)

## Client IP in Firewall Settings for SQL Server
To add the **IP address** of the client you access the database from, do the following steps:
* Under the SQL Server Settings, click on **Firewall**.
* Click on **Add client IP** and click **Save**. This will add your current IP as a new rule on the Firewall.

  ![Screenshot](media/app-service/appmod-pic-0116.png)

## Publish the Database into Azure SQL DB
* From Visual Studio, expand the project **Contoso.Expenses.Database**.
* Click on the **Seed.sql** file under the **Scripts folder**, and look at content in the preview window.
  * Note: This file will get executed post-deployment and add test data to the database.

  ![Screenshot](media/app-service/appmod-pic-0118.png)

* **Right-click** on the project **Contoso.Expenses.Database** and select **Publish** to publish the database.

  ![Screenshot](media/app-service/appmod-pic-0120.png)
  ![Screenshot](media/app-service/appmod-publishdatabasescreen.png)
  
* On **Target database connection** click **Edit**, then click **Browse**.

* From the list presented, expand **Azure**.
* Select the **Database** created on the SQL Server in the previous steps.

  ![Screenshot](media/app-service/appmod-pic-0122.png)

* For the **Server Name**, confirm or enter the server name previously saved to the clipboard (e.g. **contosoexpensesdbserver1.database.windows.net**).
* Set **Authentication** to **SQL Server Authentication**.
* Enter the database server **User Name** and **Password**. Select the checkbox **Remember Password**.
* Select the database name **contosoexpensesdb**.
* Click **Test Connection**.
* You may get prompted to add a firewall rule so that your computer is allowed to access the database server. If so, click **Ok** to allow the firewall rule to be created.

  ![Screenshot](media/app-service/appmod-pic-0123.png)
* The result should be **Test connection succeeded**, then click **Ok**.
* Click **Ok** to close the connect window.

* Click on **Publish** to publish the database to Azure.
  * The database will be published to Azure and give you the results.

  ![Screenshot](media/app-service/appmod-pic-0124.png)

* The status and progress will be displayed in the Data Tools Operations window.

  ![Screenshot](media/app-service/appmod-pic-0125.png)

## View the database using Visual Studio Tools
* From **Visual Studio**, select **View** | **SQL Server Object Explorer**.
* Expand the **SQL Server** node to view the connection.
* Expand the connection **contosoexpensesdbserver1.database.windows.net**, and then **Databases | contosoexpensesdb | Tables**. Confirm the existence of the **dbo.Expense** table.

  ![Screenshot](media/app-service/appmod-pic-0130.png)

## Publish the Web App

* From Visual Studio, **right-click** on the Web Project, **Contoso.Expenses.Web**.
* Select **Publish** from context menu, to pick a new publish target.

  ![Screenshot](media/app-service/appmod-pic-0132.png)

* Click on **Create new Profile** from the **Publish dialog**.

  ![Screenshot](media/app-service/appmod-pic-0134.png)

* Create a new **Microsoft Azure App Service**, click **Ok**.

  ![Screenshot](media/app-service/appmod-pic-0136.png)

## Create App Service
* Login into the correct **Subscription** from the dropdown list.
* Set the **Web App Name** to any name, e.g. **ContosoExpenesesWeb20170710012420**
* Select the correct **Subscription** from the dropdown list.
* Select the correct **Resource Group** from the dropdown list, e.g. **ContosoExpenses-RG-WestUS2**.

  ![Screenshot](media/app-service/appmod-pic-0138.png)

* For the App Service Plan, click **New**.
* To configure the app service plan, enter a name or use the default. e.g. **ContosoExpensesWeb20170710012420Plan**.
* For **Location**, select your location. e.g. **West US 2**.
* For **Size**, select **S1 (1 core, 1.75 GB RAM)**.
* Click **OK**.

  ![Screenshot](media/app-service/appmod-pic-0139.png)

* Make sure the correct **App Service Plan** is selected in the dropdown list. e.g. **ContosoExpensesWeb20170710012420Plan**.
* Click **Create**.

  ![Screenshot](media/app-service/appmod-pic-0140.png)

* Once the **App Service Plan** is created, click on **Publish**.

  ![Screenshot](media/app-service/appmod-pic-0141.png)

  * The output window will give you progress of the deployment.

  ![Screenshot](media/app-service/appmod-pic-0142.png)
  
  * At the end of the deployment, Visual Studio should automatically launch your default browser to show your published web app.

## Launch the website from the Azure Portal
* Navigate to the **Azure portal**.
* Click on **Resource Group**, select **ContosoExpenses-RG-WestUS2**.
* Click on the **App Service name** you deployed, e.g. **ContosoExpensesWeb20170710012420**.

  ![Screenshot](media/app-service/appmod-pic-0143.png)

* Click on **Overview** then the **URL** to launch the website.

  ![Screenshot](media/app-service/appmod-pic-0144.png)
  
  ![Screenshot](media/app-service/appmod-pic-0146.png)

## Update the Connection String in App Settings
* First, let's capture the database server name to the clipboard.
  * Go back to the list of **Resource Groups**.
  * Click on **ContosoExpenses-RG-WestUS2**.
  * Click on **contosoexpensesdbserver**.
  * Click on **Properties**.
  * Highlight the database Server Name and **right-click** to select **Copy** to save it on the clipboard.

  ![Screenshot](media/app-service/appmod-pic-0150.png)

* Navigate back to your **Resource Group**.
* Click on the **App Service name** you deployed, e.g. **ContosoExpensesWeb20170710012420**.
* From the Settings area, click on **Application settings**.
* On the **Connection strings** area, type **ContosoExpensesDataEntities** as the connection string name.

  ![Screenshot](media/app-service/appmod-pic-0152.png)

* Provide the connection string value in Connection Strings in the portal for the WebApp.
  * Note: Modify the following <**marked**> items with your server name, database name and login credentials, used in above steps.
  * Hint: **Copy** the following connection string into **Notepad**, then replace the 4 bolded areas with your information.
  
   ```
    metadata=res://*/Models.ContosoExpensesModel.csdl|res://*/Models.ContosoExpensesModel.ssdl|res://*/Models.ContosoExpensesModel.msl;provider=System.Data.SqlClient;provider connection string="data source=tcp:contosoexpensesdbserver1.database.windows.net;initial catalog=contosoexpensesdb;Integrated Security=False;User Id=Username;Password=Password;MultipleActiveResultSets=True;App=EntityFramework"
   ```

   ![Screenshot](media/app-service/appmod-pic-0154.png)

* **Copy** the new connection string from Notepad into the clipboard.
* **Paste** into the **Value** textbox, alongside of the name **ContosoExpensesDataEntities** entered above.
* Select **Custom**, from the dropdown list.
* Click on **Save**.

    ![Screenshot](media/app-service/appmod-pic-0156.png)

## Deploy the API app
* From Visual Studio, **right-click** on the Web Project, **Contoso.Expenses.API**.
* Select **Publish** from context menu, to pick a new publish target.

  ![Screenshot](media/app-service/appmod-pic-0160.png)

* Click on **Create new Profile** from the **Publish dialog**.

  ![Screenshot](media/app-service/appmod-pic-0162.png)

* Create a new **Microsoft Azure App Service**, click **Ok**.

  ![Screenshot](media/app-service/appmod-pic-0136.png)

## Create App Service
* Login into the correct **Subscription** from the dropdown list.
* Set the **API App Name** to any name, e.g. **contosoexpensesapi20170711011807**
* Select the correct **Subscription** from the dropdown list.
* Select the correct **Resource Group** from the dropdown list, e.g. **ContosoExpenses-RG-WestUS2**.
* Select the correct **App Service Plan** from the dropdown list, e.g. **ContosoExpensesWeb20170710012420Plan** from a previous step.
* Click **Create**.

  ![Screenshot](media/app-service/appmod-pic-0164.png)

* Once the **App Service Plan** is created, click on **Publish**.

  ![Screenshot](media/app-service/appmod-pic-0166.png)

  * The output window will give you progress of the deployment.

  ![Screenshot](media/app-service/appmod-pic-0168.png)
  
  * At the end of the deployment, Visual Studio should automatically launch your default browser to show your published web app.

## Update the App Service Settings
* From the Azure Portal, click on **Resource Groups**.
* Click on **ContosoExpenses-RG-WestUS2**.
* Click on the **App Service** you deployed for the API app, e.g. **ContosoExpensesAPI20170711011807**.
* Click on **Overview** and copy the URL to the clipboard.

  ![Screenshot](media/app-service/appmod-pic-0170.png)

* Go back to the **Resource Group** and now select the **App Service** for the Expense web app, e.g. **ContosoExpensesWeb20170710012420**.
* From the **Settings area**, click on **Application Settings**.
* Under **App settings**, add two new entries in the first and second textboxes.
  * Type **EmployeeName** and <**your name**>.
  * Type **EmployeeApiUri** and paste in the URL of the API app, e.g. **http://contosoexpensesapi20170711011807.azurewebsites.net**
* Click **Save**.

  ![Screenshot](media/app-service/appmod-pic-0172.png)

## Create a Storage Account
* From the Azure Portal, click on **Resource Groups**, **ContosoExpenses-RG-WestUS2**.
* Click on the **+ Add**, type **Storage** and press **Enter**.
* Click on **Storage Account  - blob, file, table, queue**.
* Click on **Create**.

  ![Screenshot](media/app-service/appmod-pic-0180.png)

* Type in **contosoexpensesUNIQUEIDsa** (e.g. **contosoexpenses123sa**) for the **Name**. The name of a storage account needs to be globally unique.
* If needed, change **Replication** to **Locally-redundant storage (LRS)**.
* Make sure you select the correct **Resource Group** and **Location** as before.
* Leave other fields with default options.
* Click on **Create**.

  ![Screenshot](media/app-service/appmod-pic-0182.png)

## Update the web.config for Storage Account
* From the Azure Portal, click on **Resource Groups**, **ContosoExpenses-RG-WestUS2**.
* Click on the **Storage Account** you deployed, e.g. **contosoexpensessa**.
* Under **Settings**, click on **Access keys**.

  ![Screenshot](media/app-service/appmod-pic-0186.png)

* From **Key1**, copy the **Key** to the clipboard, paste into a Notepad.
* From **Key1**, copy the **Connection** to the clipboard, paste into a Notepad.

  ![Screenshot](media/app-service/appmod-pic-0188.png)
  
* From **Visual Studio**, open up the **web.config** under **Contoso.Expenses.Web**.
* Scroll down and locate **StorageConnectionString**, then paste the connection string from the clipboard between the quotes. Save the changes on the file.

  ![Screenshot](media/app-service/appmod-pic-0190.png)

## Setup a Sendgrid Email Account
* From the Azure Portal, click on **Resource Groups**, **ContosoExpenses-RG-WestUS2**.
* Click on the **+ Add**, type **SendGrid** and press **Enter**.
* Click on **SendGrid Email Delivery**.
* Click on **Create**.

  ![Screenshot](media/app-service/appmod-pic-0194.png)

* Type **DemoSendGridEmail** for the **Name**.
* Type in a **Password**.
* Select the **Subscription**.
* Select the **Use existing** for the resource group.
* Click on **Pricing Tier**, select **F1 Free**.
* Click **Select**.

  ![Screenshot](media/app-service/appmod-pic-0196.png)

* Click on **Contact Information**.
* Enter **First Name**, **Last Name**, **Email**, **Company**.
* Click **Ok**.

  ![Screenshot](media/app-service/appmod-pic-0198.png)

* Click on **Legal Terms**, view, then click **Purchase**.

  ![Screenshot](media/app-service/appmod-pic-0200.png)

* Click **Create**, to create the SendGrid Email Account.
  * Note down the USERNAME from the Configuration blade. We need the UserName & Password to login to SendGrid site to generate API Key.
  ![Screenshot](media/app-service/appmod-pic-0202.png)

* Using a **Microsoft Edge or Google Chrome browser**, navigate to **http://app.sendgrid.com**.
* Login using the **Username** &  **Password** you created earlier.
  * You will see the SendGrid dashboard/portal.
  ![Screenshot](media/app-service/appmod-pic-0224.png)

* From the left-hand panel, click on **Settings** | **API Keys**.
* Click the button **Create API Key**.
  ![Screenshot](media/app-service/appmod-pic-0225.png)

* Type in **SendGridAPIKey** for the **API Key Name**.
* Select **Full Access**, then click **Create & View**.
  ![Screenshot](media/app-service/appmod-pic-0226.png)

* Click on the generated key and save it in Notepad. We will use this key in the Functions AppSettings.
  * Note: This key will **NOT** be available to copy after this blade is closed.
* Click **Done**.
    ![Screenshot](media/app-service/appmod-pic-0227.png)


## Create an Azure Function

* From Visual Studio, **right-click** on the Web Project, **Contoso.Expenses.Functions**.
* Select **Publish** from context menu, to pick a new publish target.

* Click on **Create new Profile** from the **Publish dialog**.

* In the **Pick a publish target** dialog, select **Create New** and click **OK**

* In the **Create App Service** dialog box, enter the following:
  * Unique function App Name, example **ContosoExpensesEmailFunction**
  * Select your subscription
  * Resource Group: Select appropriate existing Resource Group
  * App Service Plan: Click New and select **Size** as **Consumption**
  ![Screenshot](media/app-service/appmod-pic-0253.png)  

  ![Screenshot](media/app-service/appmod-pic-0254.png)

  * Click **Create**

* Once the **App Service Plan** is created, click on **Publish**.

  ![Screenshot](media/app-service/appmod-pic-0255.png)

* The output window will give you progress of the deployment. Make sure its successfully published.

* Launch Azure Portal and navigate to **Function Apps**

* Select your function and click on **Application Settings**
  ![Screenshot](media/app-service/appmod-pic-0256.png)

* Under **Applications Settings**, add the following two settings and appropriate values from above. 
  * **StorageConnectionString**
  * **AzureWebJobsSendGridApiKey**

  ![Screenshot](media/app-service/appmod-pic-0257.png)

* Restart the function
![Screenshot](media/app-service/appmod-pic-0258.png)

## Testing

* If you have an expense message already sitting in the storage queue, the function will automatically trigger and send an email. Check your inbox, it might take up to 5 minutest to receive the email.

* Alternatively you can also create a new expense by going through the web app. 
![Screenshot](media/app-service/appmod-pic-0259.png)

* Once you create an expense successfully, if your function is stopped, you can head to the **Storage account** and check the **Queue**, a message will be sitting there. 
![Screenshot](media/app-service/appmod-pic-0260.png)

* If you start the function, it should pick up the message and email will go out.
![Screenshot](media/app-service/appmod-pic-0261.png)


## Review the Contoso.Expenses.API app
* In Visual Studio, select the project **Contoso.Expenses.API**.
* Expand the **Controllers folder**, click on **EmployeeController.cs** file.
  * On line **#35** you will see name **Randy**.
  * Feel free to modify this for your own user name that you have entered in the web app's application settings before.
* Right-click on the project **Contoso.Expenses.API**, select **Publish**.
* Verify the publish profile is the one previously created (e.g. **contosoexpensesapi20170711011807**), and click **Publish**.
* Once published, copy the URL in the output folder and paste it into a browser. e.g. **http://contosoexpensesapi20170711011807.azurewebsites.net**.

## Testing out the API with Swagger
* In the browser, add **/swagger** to the end of the URL, and press enter.
  * e.g. **http://contosoexpensesapi20170711011807.azurewebsites.net/swagger**
  * This will load the Swagger page that will allow you to explore and test your APIs.
* Click on **Employee**, and then click on **GET**.
* Under **Parameters,** type in **Randy** for the **employeeName** parameter and then click **Try it Out!**

  ![Screenshot](media/app-service/appmod-pic-0240.png)

  * The response body will return **ManagerName** with a response code of 200.

  ![Screenshot](media/app-service/appmod-pic-0241.png)

## Test out the Website
* From Visual Studio, right-click on the Web Project, **Contoso.Expenses.Web**.
* Select **Publish** from context menu, to pick a new publish target.
* Select the previously used Profile from the Publish dialog and click on **Publish**.
  * This publishes the change to the **Web.config** file to use the correct **StorageConnectionString**.
  * Alternatively, you could update the setting directly in the Azure portal for the web app, under the **Application settings** as before.
* From the Azure Portal, click on **Resource Groups**.
* Click on **ContosoExpenses-RG-WestUS2**.
* Click on the **App Service** you deployed, e.g. **ContosoExpensesWeb20170710012420**.
* Click on URL to launch the website. e.g. **http://contosoexpensesweb20170710012420.azurewebsites.net/**.
* From the website, click on **Expenses**.
* Click on **Create New**.

  ![Screenshot](media/app-service/appmod-pic-0244.png)

* Type in values for a test record and click **Create**.
  ![Screenshot](media/app-service/appmod-pic-0245.png)
  ![Screenshot](media/app-service/appmod-pic-0246.png)

## View the Queue from Visual Studio
* From Visual Studio, select the menu item **View** | **Cloud Explorer**.
* Select the correct subscription, and click **Apply**.

  ![Screenshot](media/app-service/appmod-pic-0250.png)

* Expand the nodes to **Storage Accounts** | YourStorageAccountName (e.g. contosoexpensessa) | **Queues**.
* Right-click on **contosoexpenses** and select **Open Queue Editor**.
  * Note: The email message has been processed and de-queued.

    ![Screenshot](media/app-service/appmod-pic-0252.png)

## Useful References
* [Visual Studio Team Services, Getting Started](https://www.visualstudio.com/)
* [Visual Studio Team Services, Product Updates](https://www.visualstudio.com/team-services/updates/)
* [Visual Studio Team Services, Support](https://www.visualstudio.com/team-services/support/)
