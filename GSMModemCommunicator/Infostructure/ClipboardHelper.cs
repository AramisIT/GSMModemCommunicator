using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GSMModemCommunicator.Infostructure
    {
    internal class ClipboardHelper
        {
        private static volatile ClipboardHelper instance;
        private static object locker = new object();
        private SynchronizationContext synchronizationContext;

        private ClipboardHelper()
            {
            }

        public static ClipboardHelper Instance
            {
            get
                {
                if (instance == null)
                    {
                    lock (locker)
                        {
                        if (instance == null)
                            {
                            instance = new ClipboardHelper();
                            }
                        }
                    }
                return instance;
                }
            }

        public void Init(SynchronizationContext synchronizationContext)
            {
            this.synchronizationContext = synchronizationContext;
            }

        public string GetClipboardText()
            {
            string text = null;
            synchronizationContext.Send((obj) =>
                {
                    try
                        {
                        text = Clipboard.GetText();
                        }
                    catch { }
                }, null);

            return text;
            }

        public bool SetClipboardText(string text)
            {
            bool result = false;
            synchronizationContext.Send((obj) =>
            {
                try
                    {
                    if (string.IsNullOrEmpty(text))
                        {
                        Clipboard.Clear();
                        result = string.IsNullOrEmpty(Clipboard.GetText());
                        }
                    else
                        {
                        Clipboard.SetText(text);
                        result = text.Equals(Clipboard.GetText());
                        }
                    }
                catch { }
            }, null);

            return result;
            }
        }
    }
