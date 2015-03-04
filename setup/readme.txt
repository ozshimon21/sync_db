© 2009 Microsoft Corporation.  All rights reserved.


WebSharingAppDemo - End-To-End SharingAppDemo Sample Application

This application demonstrates how to use Sync Framework database synchronization providers to configure and execute peer-to-peer synchronization between a SQL Server database and one or more SQL Server Compact databases via a remote WCF service.

What is Demonstrated in This Sample?

- Synchronizing a server database scope (hosted in a SQL Server or SQL Server Express instance) with multiple instances of a Compact client database.
- Enabling synchronization over an n-tier model by using WCF as an endpoint.
- The new multi-scope change-tracking model on the server.
- Two ways to configure and synchronize a Compact database: full initialization and snapshot initialization.
- Configuration of SqlSyncProvider and SqlCeSyncAdapter objects.
- Enabling batched synchronization.
 

How Do I Install the Application?

1- Connect to an instance of SQL Server, and open and execute peer1_setup.sql.
2- Open demo.sql and execute the "Insert Sample Data In Tables" sections at the top of the script.
3- Install WCF components - Refer to http://msdn.microsoft.com/en-us/library/aa751792.aspx section  "Ensure That IIS and WCF Are Correctly Installed and Registered" to install and configure .Net 3.0.
4- In Visual Studio, open the WebSharingAppDemo-CEProviderEndToEnd solution with ADMIN privileges. Admin privileges are necessary to start the self hosted WCF service sample.
5- Build the project.


What do the Individual CS Files Contain? 

App directory - Contains all the code files for the Windows Form app.
 App\CeCreationUtilities.cs - Contains utility classes that the app uses to handle string constants and hold client database information.
 App\CESharingForm.cs - Main entrance point for the Windows Form app. Contains all GUI eventing/OnClick logic. 
 App\NewCEClientCreationWizard.cs - New wizard app that is used to gather user information to configure and provision a new Compact client database.
 App\ProgressForm.cs - Form app that shows progress information for each SyncOrchestrator.Synchronize() call.
 App\Resource.resx and App\Resources.Designer.cs - Resource files. 
 App\SharingApp.cs - Contains the Main function that launches a new instance of the CESharingForm class.
 App\SynchronizationHelper.cs - The main class that handles configuration of server side SqlSyncProvider and client side SqlCeSyncProvider instances. Short instructions are included for each method in the class:
	CheckAndCreateCEDatabase() - Utility function that creates an empty Compact database.
	CheckIfProviderNeedsSchema() - Sample that demonstrates how a Compact provider would determine if the underlying database needs a schema or not.
	ConfigureCESyncProvider() - Sample that demonstrates how to configure SqlCeSyncProvider.
	ConfigureSqlSyncProvider() - Sample that demonstrates how to configure SqlSyncProvider.
	provider_*() - Sample client side event registration code that demonstrates how to handle specific events raised by Sync Framework.
 App\TablesViewControl.cs - Custom user control that displays values from the two sample tables (orders and order_details), based on the client and server connections that are passed in. 

WebSyncContract directory - Contains all files related to the WCF service. Some important files are
	WebSyncContract\WebSyncContract\IRelationalSyncContract.cs - Interface declaring the base WCF service and operations.
	WebSyncContract\WebSyncContract\ISqlSyncContract.cs - Interface declaring the Sql Server specific operations such as GetSchema.
	WebSyncContract\WebSyncContract\ICESyncContract.cs - Interface declaring the Ce client specific service operations such as GenerateSnapshot and CreateSchema.
	WebSyncContract\WebSyncContract\RelationalWebSyncService.cs - Implementation of IRelationalSyncContract.
	WebSyncContract\WebSyncContract\SqlWebSyncService.cs - Implementation of ISqlSyncContract.
	WebSyncContract\WebSyncContract\CeWebSyncService.cs - Implementation of ICeSyncContract.	
	WebSyncContract\WebSyncContract\RelationalProviderProxy.cs - Base WCF Proxy class implementation.
	WebSyncContract\WebSyncContract\SqlSyncProviderProxy.cs - Sql Server specific Proxy class implementation.
	WebSyncContract\WebSyncContract\CeSyncProviderProxy.cs - Ce specific Proxy class implementation.
	WebSyncContract\WebSyncContract\WebServiceHostLauncher.cs - Simple class that self-hosts the WCF services.
	WebSyncContract\WebSyncContract\App.config - Contains information on WCF endpoints and binding informations.
 
Setup directory - Contains the server provisioning .sql files.


How Do I Use the Sample?

1.  Install the application as described in the "How Do I Install" section.
2.  Run the sample app. By default it assumes that SQL Server is installed as localhost. If it's not, please replace Environment.MachineName with the correct instance name in SqlSharingForm.SqlSharingForm_Shown().  It also launches the self-hosted WCF service.
3.  If the sample is correctly installed, values from the orders and order_details tables should display in the datagrid on the "Server" tab.
4.  The Synchronize button is disabled until at least one Compact client is added. Add a new Compact client database by clicking "Add CE Client". Options for creating a new client in the New CE Creation Wizard:
	* Full initialization - Create an empty Compact database, get the schema from the server, create the schema on the client, and get all data from the server on the first Synchronize() call.
	* Snapshot initialization - Export an existing Compact database, initialize that snapshot, and receive only incremental changes from the server on the first Synchronize() call.
5a. For full initialization, select the location and file name. 
5b. For snapshot initialization, select the .sdf file of an existing client (in the same scope) and pick the location and name for the exported database.
6.  On clicking OK, a new tab with the name "Client#" should be added to the main form. After the client is synchronized, clicking that tab displays values for the tables orders and order_details in that Compact database.
7.  Batching can be enabled by setting a non-zero value in the Batch size text box. By default batching is disabled.
7a. The Batching location can be modified by clicking on the Change button.
8.  To synchronize, select source and destination providers, and click Synchronize.
9.  Make changes to the server or client database tables and then try to synchronize the peers to confirm that changes are synchronized.

How do I Debug the sample?

1. Ensure that the WCF service host project is configured to start.
	Open the solution Properties. Select the Startup Project pane under Common Properties in the left hand side list. Ensure that "Multiple Startup Projects" is selected, and set Action to "Start" for the WebSyncContract and WebSharingAppDemo-CEProviderEndToEnd projects.
	Ensure that WebSyncContract is started before the WebSharingAppDemo-CEProviderEndToEnd project
2. Ensure that the service is started and running.
	Right-click the "WCF Service Host" icon in the system tray and ensure that WebSyncContract.SqlWebSyncService and WebSyncContract.CeWebSyncService status report "Started". If not, then ensure that the VS project is run under Admin rights.
	
	If VS is running with the appropriate priveleges, then check to ensure that WCF is installed and configured properly. Refer to the "How Do I Install" section.
	
	Ensure that port 8000 is unblocked in your Firewall.
	
	You can also ensure the service is running by opening a browser and navigating to these links: http://localhost:8000/RelationalSyncContract/SqlSyncService/ or http://localhost:8000/RelationalSyncContract/CeSyncService/. You should see the default page showing the "You have created a service" message.