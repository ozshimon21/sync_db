using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using DbService.Client;
using DbService.Server;

namespace DbService
{
    public class WebServiceHostLauncher
    {

        private static ServiceHost sqlHost;
        private static ServiceHost ceHost;

        public static void OpenServices()
        {
            try
            {
                sqlHost = new ServiceHost(typeof(SqlWebSyncService));
                ceHost = new ServiceHost(typeof(SqlCeWebSyncService));
                sqlHost.Open();
                ceHost.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in opening servicehost. " + e);
                Console.ReadLine();
            }
        }

        public static void CloseServices()
        {
            try
            {
                if (sqlHost.State == CommunicationState.Opened ||
                    sqlHost.State == CommunicationState.Opening ||
                    sqlHost.State == CommunicationState.Created)
                {
                    sqlHost.Close();
                }

                if (ceHost.State == CommunicationState.Opened ||
                    ceHost.State == CommunicationState.Opening ||
                    ceHost.State == CommunicationState.Created)
                {
                    ceHost.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in opening servicehost. " + e);
                Console.ReadLine();
            }
        }
    }
}
