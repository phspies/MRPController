using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DoubleTakeProxyService.DimensionData.restMethods
{
    class httpPost
    {
        public String postData;
        public String responseData;
        public Int32 responseCode;
        public String responseMessage;
        public String url;
        public int timeout = 60000;
        public String httpMethod;
        public String restFunction = "";
        System.Net.HttpWebRequest myHttpWebRequest = null;
        System.Net.HttpWebResponse myHttpWebResponse = null;

        public void doPost()
        {
            String fullurl = Properties.Settings.Default.targetPlatformUrl + url;
            myHttpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(fullurl);
            Debug.Print(fullurl);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((s, ce, ch, ssl) => true);
                myHttpWebRequest.Accept = "*/*";
                myHttpWebRequest.AllowAutoRedirect = false;
                myHttpWebRequest.UserAgent = "cloudmanage/0.1";
                myHttpWebRequest.Timeout = timeout;
                IWebProxy proxy = WebRequest.GetSystemWebProxy();
                if (proxy != null)
                {
                    if (!proxy.IsBypassed(new Uri(fullurl)))
                    {
                        myHttpWebRequest.Proxy = proxy;
                    }
                }
                myHttpWebRequest.ContentType = "text/xml; encoding='utf-8'";
                myHttpWebRequest.Credentials = new NetworkCredential(Properties.Settings.Default.targetPlatformUsername, Properties.Settings.Default.targetPlatformPassword);

                if (httpMethod == "POST")
                {
                    myHttpWebRequest.Method = "POST";
                    // Use UTF8Encoding instead of ASCIIEncoding for XML requests:
                    System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                    byte[] postByteArray = encoding.GetBytes(postData);
                    myHttpWebRequest.ContentLength = postByteArray.Length;
                    System.IO.Stream postStream = myHttpWebRequest.GetRequestStream();
                    postStream.Write(postByteArray, 0, postByteArray.Length);
                    postStream.Close();

                    myHttpWebResponse = (System.Net.HttpWebResponse)myHttpWebRequest.GetResponse();
                    if (myHttpWebResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        System.IO.Stream responseStream = myHttpWebResponse.GetResponseStream();
                        System.IO.StreamReader myStreamReader = new System.IO.StreamReader(responseStream);
                        responseData = myStreamReader.ReadToEnd();
                    }
                    myHttpWebResponse.Close();
                    responseCode = Convert.ToInt32(myHttpWebResponse.StatusCode);

                }
                else
                {
                    //Get Response
                    myHttpWebRequest.Method = "GET";
                    myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    responseData = new StreamReader(myHttpWebResponse.GetResponseStream()).ReadToEnd();
                    myHttpWebResponse.Close();
                    responseCode = Convert.ToInt32(myHttpWebResponse.StatusCode);
                }
            }
            catch (WebException ex)
            {
                Debug.Print(ex.Message);
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        responseMessage = response.StatusDescription;
                        responseCode = (int)response.StatusCode;
                    }
                    else
                    {
                        responseMessage = ex.Message;
                    }
                }
                else
                {
                    responseMessage = ex.Message;
                }
            }
        }

    }
}
