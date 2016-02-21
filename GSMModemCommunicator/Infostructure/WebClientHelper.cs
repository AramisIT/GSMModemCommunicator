using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GSMModemCommunicator.Infostructure
    {
    public class WebClientHelper
        {
        private string url;
        private string content;

        public WebClientHelper(string url)
            {
            this.url = url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) ? url :
             "http://" + url;
            }

        public WebClientHelper AddContent(object _content)
            {
            this.content = (_content ?? new object()).ToString();

            return this;
            }

        public bool PerformPostRequest(out string result)
            {
            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var addContent = !string.IsNullOrEmpty(content);

            byte[] byteArray = addContent ? Encoding.ASCII.GetBytes(content) : new Byte[0];
            request.ContentLength = byteArray.Length;

            WebResponse response = null;
            try
                {
                if (addContent)
                    {
                    using (var sourceStream = request.GetRequestStream())
                        {
                        sourceStream.Write(byteArray, 0, byteArray.Length);
                        sourceStream.Close();
                        }
                    }

                response = request.GetResponse();
                }
            catch (Exception exp)
                {
                Trace.WriteLine(
                    string.Format(@"Error on authorization check: {0}. Error type is ""{1}""", exp, exp.GetType().Name));

                result = string.Format(@"Error: {0}", exp);
                return false;
                }

            using (var resultStream = response.GetResponseStream())
                {
                using (var reader = new StreamReader(resultStream))
                    {
                    string responseFromServer = reader.ReadToEnd();

                    result = responseFromServer;
                    return true;
                    }
                }
            }
        }
    }
