using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using GSMModemCommunicator.Infostructure;
using GSMModemCommunicator.Notifications;

namespace GSMModemCommunicator
    {
    class TextMessagesSenderApplicationContext : ApplicationContext
        {
        private NotifyIcon trayIcon;
        private static TextMessagesSenderApplicationContext content;

        public TextMessagesSenderApplicationContext()
            {
            content = this;
            trayIcon = new NotifyIcon();
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            trayIcon.Visible = true;
            trayIcon.ContextMenu = new TrayMenu().BuildMenu();
            }

        public static void CloseApplication()
            {
            content.ExitThreadCore();
            }

        internal void OnApplicationIdle(object sender, System.EventArgs e)
            {
            Application.Idle -= OnApplicationIdle;

            UserNotificator.Instance.Init(SynchronizationContext.Current);
            ClipboardHelper.Instance.Init(SynchronizationContext.Current);
            new Thread(() =>
                {
                    var settingsInitializator = new SettingsInitializator();
                    if (settingsInitializator.CheckNecessarySettings())
                        {
                        new MessagesSender(settingsInitializator.Settings).Run();
                        }
                }) { IsBackground = true }.Start();
            }

        internal void OnExit(object sender, EventArgs e)
            {
            trayIcon.Visible = false;
            }
        }
    }
