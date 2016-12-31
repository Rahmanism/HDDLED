using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDDLED
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Create an instance of HddActivityLed Object and start it
            var hddActivityLed = new HddActivityLed();
            hddActivityLed.Start();

            Application.Run();
        }

    }
}
