using System;
using System.Linq;
using System.ServiceModel;
using WcfService1;

namespace WcfHost1
{
    class Program
    {
        static void Main(string[] args)
        {        
            try
            {
               using (var host = new ServiceHost(typeof(Service1)))
                {
                    host.Open();
                    Console.WriteLine("The service is ready at {0}", host.Description.Endpoints[0].Address);
                    Console.ReadLine();
                    host.Close();
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    Console.WriteLine(e.Message + " InnerException:" + e.InnerException != null ? e.InnerException.Message : "");
                else
                    Console.WriteLine(e.Message);
            }
            Console.ReadLine();
        }

    }
}
