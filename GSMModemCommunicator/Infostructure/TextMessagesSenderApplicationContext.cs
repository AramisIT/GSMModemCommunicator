using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace GSMModemCommunicator
    {
    class TextMessagesSenderApplicationContext : ApplicationContext
        {
        private NotifyIcon trayIcon;

        public TextMessagesSenderApplicationContext()
            {
            trayIcon = new NotifyIcon();
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            trayIcon.Visible = true;
            trayIcon.ContextMenu = new TrayMenu(base.ExitThreadCore).BuildMenu();
            }

        internal void OnApplicationIdle(object sender, System.EventArgs e)
            {

            }

        internal void OnExit(object sender, EventArgs e)
            {
            trayIcon.Visible = false;
            }
        }
    }
