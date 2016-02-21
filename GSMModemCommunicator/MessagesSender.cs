using GSMModemCommunicator.Infostructure;
using GSMModemCommunicator.Notifications;
using System;
using System.Diagnostics;
using System.Threading;

namespace GSMModemCommunicator
    {
    class MessagesSender
        {
        private MessagesSendingSettings settings;

        private static volatile bool applicationIsAboutToClose;

        public MessagesSender(MessagesSendingSettings messagesSendingSettings)
            {
            this.settings = messagesSendingSettings;
            }

        public static bool ApplicationIsAboutToClose
            {
            set { applicationIsAboutToClose = value; }
            }

        private Stopwatch can_tGetNewTaskErrorDuration = new Stopwatch();

        private const int MAX_SILENCE_TIME_IF_ERROR_MINUTES = 15;

        internal void Run()
            {
            var gsmCommunicator = new GsmCommunicator(settings.ModemSerialPortNumber, settings.SimCardPinCode);
            if (!string.IsNullOrEmpty(gsmCommunicator.ErrorMessage))
                {
                UserNotificator.Instance.ShowErrorWithRemoveSettingsOption(string.Format(
@"Проблема с подключением к модему: {0}.", gsmCommunicator.ErrorMessage));
                return;
                }

            can_tGetNewTaskErrorDuration.Start();

            while (true)
                {
                if (applicationIsAboutToClose) return;

                SendingTask sendingTask;
                bool errorOccurred;
                if (getNewTask(out sendingTask, out errorOccurred))
                    {
                    var success = gsmCommunicator.SendSMS(sendingTask.Message.Number, sendingTask.Message.MessageBody);
                    notifySendingResult(sendingTask.TaskId, success);
                    if (success)
                        {
                        can_tGetNewTaskErrorDuration.Restart();
                        continue;
                        }
                    if (!string.IsNullOrEmpty(gsmCommunicator.ErrorMessage))
                        {
                        showMessage(string.Format(@"Ошибка отправки сообщения: {0}", gsmCommunicator.ErrorMessage));
                        }
                    }

                if (!errorOccurred)
                    {
                    can_tGetNewTaskErrorDuration.Restart();
                    }
                else if (can_tGetNewTaskErrorDuration.Elapsed.TotalMinutes > MAX_SILENCE_TIME_IF_ERROR_MINUTES)
                    {
                    can_tGetNewTaskErrorDuration.Restart();
                    showMessage("Нет доступа к веб приложению!");
                    }

                Thread.Sleep(1000);
                }
            }

        private void showMessage(string message)
            {
            UserNotificator.Instance.ShowWarning(message);
            }

        private void notifySendingResult(long taskId, bool success)
            {
            string result;
            new WebClientHelper(string.Format("{0}/SetTaskResult?id={1}&success={2}", settings.WebApplicationUrl, taskId, success))
                .PerformPostRequest(out result);
            }

        private bool getNewTask(out SendingTask sendingTask, out bool errorOccurred)
            {
            errorOccurred = false;
            sendingTask = null;

            string taskStr;
            if (!new WebClientHelper(string.Format("{0}/GetNewTask?id={1}", settings.WebApplicationUrl, settings.ModemId))
                    .PerformPostRequest(out taskStr))
                {
                errorOccurred = true;
                return false;
                }

            if (string.IsNullOrEmpty(taskStr)) return false;

            if (!tryParseSendingMessage(taskStr, out sendingTask))
                {
                errorOccurred = true;
                return false;
                }

            return true;
            }

        private bool tryParseSendingMessage(string taskStr, out SendingTask sendingTask)
            {
            sendingTask = null;

            var splitterPos = taskStr.IndexOf('%');
            if (splitterPos <= 0) return false;

            var taskDetailsStr = taskStr.Substring(0, splitterPos);
            var message = taskStr.Substring(splitterPos + 1);

            var taskDetails = taskDetailsStr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (taskDetails.Length < 2) return false;

            long taskId;
            if (!Int64.TryParse(taskDetails[0], out taskId)) return false;

            string phoneNumberStr = taskDetails[1];

            long phoneNumber;
            if (!Int64.TryParse(phoneNumberStr, out phoneNumber)) return false;

            if (!phoneNumberStr.StartsWith("38"))
                {
                phoneNumberStr = "38" + phoneNumberStr;
                }

            sendingTask = new SendingTask(taskId, new Message(phoneNumberStr, message));

            return true;
            }
        }
    }
