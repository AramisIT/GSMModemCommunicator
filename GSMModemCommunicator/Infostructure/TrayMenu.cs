using System;
using System.Windows.Forms;

namespace GSMModemCommunicator
    {
    class TrayMenu
        {
        private Action quitApplication;
        private NotifyIcon trayIcon;

        public TrayMenu(Action quitApplication)
            {
            this.quitApplication = quitApplication;
            }

        internal ContextMenu BuildMenu()
            {
            var menu = new ContextMenu();

            menu.MenuItems.Add("Quit").Click += (sender, e) => quitApplication();

            return menu;
            }
        }
    }
