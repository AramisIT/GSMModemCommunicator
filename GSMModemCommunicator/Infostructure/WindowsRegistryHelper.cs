using System;
using Microsoft.Win32;

namespace GSMModemCommunicator.Infostructure
    {
    public class WindowsRegistryHelper
        {
        private const string WEB_APPLICATION_URL_KEY_NAME = "url";
        private const string MODEM_ID_KEY_NAME = "modem";

        private RegistryKey appKey;

        private static volatile WindowsRegistryHelper instance;
        private static readonly object locker = new object();

        public static WindowsRegistryHelper Instance
            {
            get
                {
                if (instance == null)
                    {
                    lock (locker)
                        {
                        if (instance == null)
                            {
                            instance = new WindowsRegistryHelper();
                            }
                        }
                    }
                return instance;
                }
            }

        private WindowsRegistryHelper()
            {
            appKey = initApplicationKey();
            }

        private RegistryKey initApplicationKey()
            {
            const string keyName = @"Software\Aramis IT\GSMModemCommunicator";

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(keyName, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);

            if (registryKey == null)
                {
                Registry.CurrentUser.CreateSubKey(keyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
                registryKey = Registry.CurrentUser.OpenSubKey(keyName, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
                }

            return registryKey;
            }

        public bool IsInitiated()
            {
            string url;
            Guid modemId;
            return ReadSettings(out url, out modemId);
            }

        public bool SetUrl(string url, Guid modemId)
            {
            try
                {
                appKey.SetValue(WEB_APPLICATION_URL_KEY_NAME, url);
                appKey.SetValue(MODEM_ID_KEY_NAME, modemId);
                }
            catch
                {
                return false;
                }

            return IsInitiated();
            }

        internal void RemoveSettings()
            {
            try
                {
                appKey.DeleteValue(WEB_APPLICATION_URL_KEY_NAME);
                }
            catch { }

            try
                {
                appKey.DeleteValue(MODEM_ID_KEY_NAME);
                }
            catch { }
            }

        internal bool ReadSettings(out string url, out Guid modemId)
            {
            url = null;
            modemId = Guid.Empty;

            try
                {
                if (!Guid.TryParse(appKey.GetValue(MODEM_ID_KEY_NAME).ToString(), out modemId)) return false;

                url = appKey.GetValue(WEB_APPLICATION_URL_KEY_NAME) as string;
                return !string.IsNullOrEmpty(url) && !Guid.Empty.Equals(modemId);
                }
            catch
                {
                return false;
                }
            }
        }
    }
