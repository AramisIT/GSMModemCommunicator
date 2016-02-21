using System;

namespace GSMModemCommunicator
    {
    public class Message
        {
        public string Number { get; private set; }

        public string MessageBody { get; private set; }

        public Message(string number, string messageBody)
            {
            Number = number;
            MessageBody = messageBody;
            }
        }

    public class RecievedMessage : Message
        {
        public DateTime Date { get; private set; }

        public RecievedMessage(string number, string messageBody, DateTime date)
            : base(number, messageBody)
            {
            Date = date;
            }
        }
    }
