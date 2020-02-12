using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 

namespace TimerApp
{
    class Program
    {
        public static void Main()
        {
            // Create a Timer object that knows to call our TimerCallback
            // method once every 2000 milliseconds.
            Timer t = new Timer(TimerCallback, null, 0, 1000);
            // Wait for the user to hit <Enter>
            Console.ReadLine();
        }

        private static void TimerCallback(Object o)
        {
            // Display the date/time when this method got called.
            Console.WriteLine("In TimerCallback: " + DateTime.Now.Hour);
            // Force a garbage collection to occur for this demo.
            GC.Collect();
            if(DateTime.Now.Second==30)
            {

                //Start process, friendly name is something like MyApp.exe (from current bin directory)
                System.Diagnostics.Process.Start(System.AppDomain.CurrentDomain.FriendlyName); 
                //Close the current process
                Environment.Exit(0);
            }
        }
    }
}
