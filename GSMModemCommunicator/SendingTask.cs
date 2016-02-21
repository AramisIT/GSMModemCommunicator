using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSMModemCommunicator
    {
    class SendingTask
        {
        public Message Message { get; private set; }

        public long TaskId { get; private set; }

        public SendingTask(long taskId, Message message)
            {
            TaskId = taskId;
            Message = message;
            }
        }
    }
