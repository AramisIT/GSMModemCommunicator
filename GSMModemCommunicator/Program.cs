using System;
using System.Windows.Forms;

namespace GSMModemCommunicator
    {
    static class Program
        {
        [STAThread]
        static void Main()
            {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var context = new TextMessagesSenderApplicationContext();
            Application.Idle += context.OnApplicationIdle;
            Application.ApplicationExit += context.OnExit;

            Application.Run(context);
            }
        }
    }
