using System;
using System.Collections.Generic;

using System.Text;
using System.ServiceModel;

namespace DbServiceCommon
{
    public class WebServiceHostLauncher
    {
        public static void Main(String[] args)
        {
            try
            {
               // ServiceHost sqlHost = new ServiceHost(typeof(SqlWebSyncService));
                //ServiceHost ceHost = new ServiceHost(typeof(CeWebSyncService));
                //sqlHost.Open();
                //ceHost.Open();

                Console.WriteLine("Press <ENTER> to terminate the service host");
                Console.ReadLine();

                //sqlHost.Close();
                //ceHost.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in opening servicehost. " + e);
                Console.ReadLine();
            }
        }
    }
}
