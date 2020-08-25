using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSMModemCommunicator
    {
    class MessagesSendingSettings
        {
        public int ModemSerialPortNumber { get; set; }

        public string SimCardPinCode { get; set; }

        public long ModemId { get; set; }

        public string WebApplicationUrl { get; set; }
        }
    }
