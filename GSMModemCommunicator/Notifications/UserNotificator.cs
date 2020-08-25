using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GSMModemCommunicator.Infostructure;

namespace GSMModemCommunicator.Notifications
    {
    class UserNotificator
        {
        private static volatile UserNotificator instance;
        private static readonly object locker = new object();
        private volatile SynchronizationContext synchronizationContext;
        private volatile NotificationForm form;

        private UserNotificator() { }

        public static UserNotificator Instance
            {
            get
                {
                if (instance == null)
                    {
                    lock (locker)
                        {
                        if (instance == null)
                            {
                            instance = new UserNotificator();
                            }
                        }
                    }

                return instance;
                }
            }

        internal void Init(SynchronizationContext synchronizationContext)
            {
            this.synchronizationContext = synchronizationContext;
            }

        internal void ShowWarning(string message, string actionOption = null, Action optionAction = null)
            {
            synchronizationContext.Send((obj) => showWarning(message, actionOption, optionAction), null);
            }

        internal void ShowErrorWithRemoveSettingsOption(string message)
            {
            ShowWarning(message, "Удалить все настройки этого модема", () =>
            {
                WindowsRegistryHelper.Instance.RemoveSettings();
                TextMessagesSenderApplicationContext.CloseApplication();
            });
            }

        private void showWarning(string message, string actionOption, Action optionAction)
            {
            if (form == null || !NotificationForm.Showed)
                {
                form = new NotificationForm();
                form.MessageLabel.Text = message;
                form.Show();
                }
            else
                {
                form.MessageLabel.Text = message;
                }

            form.SetOption(actionOption, optionAction);
            }
        }
    }
