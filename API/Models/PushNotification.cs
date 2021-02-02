using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace API.Models
{
    public class PushNotification
    {

        public PushNotification(NotificationViewModel input ,string deviceId)
        {
            try
            {
                // from flutter app firebase app
                var applicationID = "AAAAgmGrSf8:APA91bFFSnovC0oxLztV3bwZYQsOTbLuEPGOIvOoxJoNix1Z2aPyuBsE89nCE_7obZTTq4ImGf8S1EwyR1i25G5EUHxSP3twzVnB0FwgvSGhYZzbr2OTei9QFIMB5ber27hP2AtbIuod";

                // from my api firebase app
                var senderId = "902308947399";

                //string deviceId = "chDDD-mQS5Kvoot36RKfnU:APA91bGNcb6yYAPzgKU2fPRTXv4_8jUvUc8c92DOh-fqz7ZWg1utfZvfbrbSyYA6SexLhzBfNQtp5Pslxu6mQUi3nlrlOrOCC_0JqXdYDQ7qwZMQyQUaVXvIV_X8swyXSzbah4_xBaqc";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";

                var data = new

                {

                    to = deviceId,

                    notification = new

                    {

                        body = input.Msg,

                        title = input.Title,

                        icon = "myicon"

                    }
                };

                var serializer = new JavaScriptSerializer();

                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                tRequest.ContentLength = byteArray.Length;


                using (Stream dataStream = tRequest.GetRequestStream())
                {

                    dataStream.Write(byteArray, 0, byteArray.Length);


                    using (WebResponse tResponse = tRequest.GetResponse())
                    {

                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {

                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {

                                String sResponseFromServer = tReader.ReadToEnd();

                                string str = sResponseFromServer;

                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                string str = ex.Message;

            }

        }

    }

    public class NotificationViewModel
    {
        public string Title { get; set; }
        public string Msg { get; set; }
        public string CustomerPhone { get; set; }
    }
}