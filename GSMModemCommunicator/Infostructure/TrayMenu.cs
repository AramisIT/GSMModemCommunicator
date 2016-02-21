using System;
using System.Threading;
using System.Windows.Forms;

namespace GSMModemCommunicator
    {
    class TrayMenu
        {
        private NotifyIcon trayIcon;

        public TrayMenu() { }

        internal ContextMenu BuildMenu()
            {
            var menu = new ContextMenu();

            menu.MenuItems.Add("Закрыть систему отправки СМС").Click += (sender, e) =>
                {
                    MessagesSender.ApplicationIsAboutToClose = true;
                    Thread.Sleep(2000);
                    TextMessagesSenderApplicationContext.CloseApplication();
                };
            //menu.MenuItems.Add("Send test message to Denis").Click += (sender, e) =>
            //    {
            //        var helper = new GsmCommunicator(3, "7384");
            //        if (!string.IsNullOrEmpty(helper.ErrorMessage))
            //            {
            //            MessageBox.Show("Не удается создать GsmCommunicator: " + helper.ErrorMessage);
            //            return;
            //            }
            //        var sent = helper.SendSMS("380508090208", "Смс модем работает!");

            //        MessageBox.Show(sent && string.IsNullOrEmpty(helper.ErrorMessage) ? "Отправлено!" : "Error: " + helper.ErrorMessage);
            //    };

            return menu;
            }
        }
    }
