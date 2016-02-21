using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GSMModemCommunicator.Infostructure;
using GSMModemCommunicator.Notifications;

namespace GSMModemCommunicator
    {
    class SettingsInitializator
        {
        public SettingsInitializator()
            {
            }

        private const string HELP_URL = "https://youtu.be/Sa8MSySxFCo";

        internal bool CheckNecessarySettings()
            {
            if (!WindowsRegistryHelper.Instance.IsInitiated())
                {
                if (!showHelpMaterials()) return false;

                Guid modemId;
                string url;
                if (!getNewModemSettingsFromClipboard(out modemId, out url))
                    {
                    UserNotificator.Instance.ShowWarning(@"Истекло время ожидания регистрации нового модема. Программа будет закрыта!");
                    return false;
                    }

                if (!saveNewModemSettings(url, modemId))
                    {
                    UserNotificator.Instance.ShowWarning(@"Не удается зарегистрировать новый модем.
Обратитесь к системному администратору!");
                    return false;
                    }

                if (!WindowsRegistryHelper.Instance.SetUrl(url, modemId))
                    {
                    UserNotificator.Instance.ShowWarning(@"Не удается сохранить настройки модема в системном реестре.
Обратитесь к системному администратору!");
                    return false;
                    }
                }

            if (!readSettings())
                {
                UserNotificator.Instance.ShowErrorWithRemoveSettingsOption(@"Не удается подключиться к системе!"); ;
                return false;
                }

            return true;
            }

        private bool readSettings()
            {
            Guid modemId;
            string url;
            if (!WindowsRegistryHelper.Instance.ReadSettings(out url, out modemId)) return false;

            string result;
            if (!new WebClientHelper(string.Format("{0}/GetSettings?id={1}", url, modemId))
                 .PerformPostRequest(out result)) return false;

            var parts = result.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3) return false;

            long modemIdInt64;
            int portNumber;
            if (!Int64.TryParse(parts[0], out modemIdInt64)
                || !Int32.TryParse(parts[1], out portNumber)) return false;

            var pinCode = parts[2];
            if (pinCode.Length != 4) return false;
            foreach (var _char in pinCode)
                {
                if (!char.IsDigit(_char)) return false;
                }

            Settings = new MessagesSendingSettings()
                {
                    ModemId = modemIdInt64,
                    ModemSerialPortNumber = portNumber,
                    SimCardPinCode = pinCode,
                    WebApplicationUrl = url,
                };

            return true;
            }

        private bool saveNewModemSettings(string url, Guid modemId)
            {
            string result;
            if (!new WebClientHelper(string.Format("{0}/SaveModem?id={1}", url, modemId))
                 .PerformPostRequest(out result)) return false;

            return result.Equals("OK");
            }

        private bool getNewModemSettingsFromClipboard(out Guid modemId, out string url)
            {
            modemId = Guid.Empty;
            url = null;

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
                {
                Thread.Sleep(400);

                if (stopWatch.Elapsed.TotalMinutes > 60) return false;

                string clipBoardText = null;

                clipBoardText = ClipboardHelper.Instance.GetClipboardText();

                if (tryParseNewModemSettings(clipBoardText, out modemId, out url))
                    {
                    ClipboardHelper.Instance.SetClipboardText("");
                    return true;
                    }
                }
            }

        private bool tryParseNewModemSettings(string clipBoardText, out Guid modemId, out string url)
            {
            const string REQUIRED_PREFIX = "%ModemSettings%";
            modemId = Guid.Empty;
            url = null;

            if (!clipBoardText.StartsWith(REQUIRED_PREFIX)) return false;

            var parameters = clipBoardText.Substring(REQUIRED_PREFIX.Length).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parameters.Length != 2 || !Guid.TryParse(parameters[0], out modemId) || Guid.Empty.Equals(modemId)) return false;

            url = parameters[1];
            return !string.IsNullOrEmpty(url);
            }

        private static bool showHelpMaterials()
            {
            try
                {
                var sInfo = new ProcessStartInfo(HELP_URL);
                Process.Start(sInfo);
                }
            catch (Exception exp)
                {
                UserNotificator.Instance.ShowWarning(string.Format(
@"Не удается открыть страницу, проверьте наличие браузера, доступ к интернету: {0}", exp.Message));
                return false;
                }

            return true;
            }

        public MessagesSendingSettings Settings { get; private set; }
        }
    }
