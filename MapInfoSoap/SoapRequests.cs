using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.Net;

using System.Security;
using System.Collections;


using System.Windows.Forms;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

using System.Linq;



namespace MapInfoSoap
{
    public class SoapRequests
    {
        // Takes address information and returns records that match the criteria best
        public static string ValidateAddressSoapRequest(string unitNumber, string buildingName, string numberFirst, string numberSuffixFirst, string numberLast, string numberSuffixLast, string roadName, string roadNameType, string roadNameSuffix, string localityName, string localGovernmentName, string stateName)
        {
            //var uri = new Uri("https://information.qld.gov.au/service/land/property/ValidationService/1/soap");
            var uri = new Uri("https://information.qld.gov.au/service/Addressing/ValidationService/PLSplus/soap");


            HttpWebRequest req = (HttpWebRequest)WebRequest.CreateDefault(uri);
            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Method = "POST";
            req.Accept = "text/xml";
            req.Headers.Add("SOAPAction", @"information.derm.qld.gov.au/service/Addressing/ValidationService/1/soap/IValidationService/ValidateAddress");


            XmlDocument soapEnv = new XmlDocument();

            soapEnv.LoadXml(@"<?xml version='1.0' encoding='utf-8'?><soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:soap='information.derm.qld.gov.au/service/Addressing/ValidationService/1/soap'><soapenv:Header/><soapenv:Body><soap:ValidateAddress><!--Optional:--><soap:unitNumberDetails>" + unitNumber + @"</soap:unitNumberDetails><!--Optional:--><soap:buildingName>" + buildingName + @"</soap:buildingName><!--Optional:--><soap:numberFirst>" + numberFirst + @"</soap:numberFirst><!--Optional:--><soap:numberSuffixFirst>" + numberSuffixFirst + @"</soap:numberSuffixFirst><!--Optional:--><soap:numberLast>" + numberLast + @"</soap:numberLast><!--Optional:--><soap:numberSuffixLast>" + numberSuffixLast + @"</soap:numberSuffixLast><!--Optional:--><soap:roadName>" + roadName + @"</soap:roadName><!--Optional:--><soap:roadNameType>" + roadNameType + @"</soap:roadNameType><!--Optional:--><soap:roadNameSuffix>" + roadNameSuffix + @"</soap:roadNameSuffix><!--Optional:--><soap:localityName>" + localityName + @"</soap:localityName><!--Optional:--><soap:localGovernmentName>" + localGovernmentName + @"</soap:localGovernmentName><!--Optional:--><soap:stateName>" + stateName + @"</soap:stateName></soap:ValidateAddress></soapenv:Body></soapenv:Envelope>");

            using (Stream stream = req.GetRequestStream())
            {
                soapEnv.Save(stream);
            }


            IAsyncResult asyncRes = req.BeginGetResponse(null, null);

            asyncRes.AsyncWaitHandle.WaitOne();

            string soapResponse;

            using (WebResponse webResponse = req.EndGetResponse(asyncRes))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResponse = rd.ReadToEnd();
                }
            }

            return soapResponse;
        }

        // Takes a single line address and returns all matching addresses in SOAP XML
        public static string ParseValidAddress(string address)
        {


            string username = "plspQ_SAMSinfo";
            string password = "SEce37f1c";
                                                
            // Construct request
            var uri = new Uri("https://information.qld.gov.au/service/Addressing/ValidationService/PLSplusQG/soap"); //endpoint

            SecurityBindingElement securityElement = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            securityElement.IncludeTimestamp = false;
            var encodingElement = new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8);
            var transportElement = new HttpsTransportBindingElement();
            var binding = new CustomBinding(securityElement, encodingElement, transportElement);
            var endpoint = new EndpointAddress("https://information.qld.gov.au/service/Addressing/ValidationService/PLSplusQG/soap");
            //var endpoint = new EndpointAddress(uri);

            var service = new ServiceReference1.PLSpQGClient(binding, endpoint);
            service.ClientCredentials.UserName.UserName = username;
            service.ClientCredentials.UserName.Password = password;

          
            MapInfoSoap.ServiceReference1.ParseAddressRequest obj  = new ServiceReference1.ParseAddressRequest();
            obj.AddressString = address;
            //obj.MeshblockOption = "@TRUE";

            string responseStr = null; 
            try
            {

            var resultsX = service.ParseAddress(obj);


                //Build string here
                
                int LoopCnt = resultsX.Results.Length;
                if (LoopCnt >= 1)
                    {
                    LoopCnt = 1; //Only return the first value
                    for (int i = 0; i < LoopCnt; i++)
                        {
                         responseStr += resultsX.Results[i].Address.RoadNumber.First.ToString();
                         responseStr += " " + resultsX.Results[i].Address.Road.Name.ToString();
                         responseStr += " " + resultsX.Results[i].Address.Road.TypeCode.ToString();
                         responseStr += " " + resultsX.Results[i].Address.Locality.ToString();
                         responseStr += "|" + resultsX.Results[i].Geocode[i].Latitude.ToString();
                         responseStr += "|" + resultsX.Results[i].Geocode[i].Longitude.ToString() + ";";

                         if (resultsX.Results[i].Confidence < 80) //If there record isn't cofident
                             {
                                 responseStr = "Error incorrect value";
                             }
                        }
                    }
                else
                {
                    responseStr = "Error incorrect value";
                }


            }
            catch (WebException e) //bad web request error
            {
                responseStr = "Too Many Results";
            }


            return responseStr;

        }

        // A test function - Successfully tested in MapInfo
        public static string GetSoap()
        {
            MessageBox.Show("Beginning soap request...");

            var uri = new Uri("https://information.qld.gov.au/service/land/property/ValidationService/1/soap");

            HttpWebRequest req = (HttpWebRequest)WebRequest.CreateDefault(uri);
            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Method = "POST";
            req.Accept = "text/xml";
            req.Headers.Add("SOAPAction", @"information.derm.qld.gov.au/service/Addressing/ValidationService/1/soap/IValidationService/ValidateAddress");


            XmlDocument soapEnv = new XmlDocument();
            soapEnv.LoadXml(@"<?xml version='1.0' encoding='utf-8'?><soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:soap='information.derm.qld.gov.au/service/Addressing/ValidationService/1/soap'><soapenv:Header/><soapenv:Body><soap:ValidateAddress><!--Optional:--><soap:unitNumberDetails>3</soap:unitNumberDetails><!--Optional:--><soap:buildingName></soap:buildingName><!--Optional:--><soap:numberFirst>100</soap:numberFirst><!--Optional:--><soap:numberSuffixFirst></soap:numberSuffixFirst><!--Optional:--><soap:numberLast></soap:numberLast><!--Optional:--><soap:numberSuffixLast></soap:numberSuffixLast><!--Optional:--><soap:roadName>webster</soap:roadName><!--Optional:--><soap:roadNameType>road</soap:roadNameType><!--Optional:--><soap:roadNameSuffix></soap:roadNameSuffix><!--Optional:--><soap:localityName>stafford</soap:localityName><!--Optional:--><soap:localGovernmentName>queensland</soap:localGovernmentName><!--Optional:--><soap:stateName></soap:stateName></soap:ValidateAddress></soapenv:Body></soapenv:Envelope>");

            using (Stream stream = req.GetRequestStream())
            {
                soapEnv.Save(stream);
            }


            IAsyncResult asyncRes = req.BeginGetResponse(null, null);

            asyncRes.AsyncWaitHandle.WaitOne();

            string soapResponse;

            using (WebResponse webResponse = req.EndGetResponse(asyncRes))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResponse = rd.ReadToEnd();
                }
            }

            return soapResponse;
        }

        // Takes a single line address and returns all matching addresses in SOAP XML
        public static string ParseLotPlan(string lotNum, string lotPlan) {

            string soapResponse;
            // Construct request
            var uri = new Uri("https://information.qld.gov.au/service/land/property/ValidationService/1/soap");

            HttpWebRequest req = (HttpWebRequest)WebRequest.CreateDefault(uri);
            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Method = "POST";
            req.Accept = "text/xml";
            req.Headers.Add("SOAPAction", @"information.derm.qld.gov.au/service/Addressing/ValidationService/1/soap/IValidationService/ValidateLotPlan");

            // Create XML body for the request
            XmlDocument soapEnv = new XmlDocument();
            soapEnv.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:soap='information.derm.qld.gov.au/service/Addressing/ValidationService/1/soap'><soapenv:Header/><soapenv:Body><soap:ValidateLotPlan><soap:lotNumber>" + lotNum + "</soap:lotNumber><soap:planNumber>" + lotPlan + "</soap:planNumber></soap:ValidateLotPlan></soapenv:Body></soapenv:Envelope>");


            using (Stream stream = req.GetRequestStream())
            {
                soapEnv.Save(stream);
            }


            IAsyncResult asyncRes = req.BeginGetResponse(null, null);

            asyncRes.AsyncWaitHandle.WaitOne();
           
            try
            {
                using (WebResponse webResponse = req.EndGetResponse(asyncRes))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResponse = rd.ReadToEnd();
                    }
                }

                //Extract values
                XDocument xD = XDocument.Load(XmlReader.Create(new StringReader(soapResponse)));

                IEnumerable<XElement> parcels =
                    (from e in xD.Root.Descendants()
                     where e.Name.LocalName == "Parcel"
                     select e);

                List<SoapAddress> addresses = new List<SoapAddress>();
                foreach (XElement p in parcels)

                {
                    SoapAddress sA = new SoapAddress(p);
                    addresses.Add(sA);
                }

                // Sort and remove duplicates
                string[] result = (from adress in addresses.OrderBy(a => a.Number).ToList() select adress.ToString()).ToArray();
                var newstring = new HashSet<string>(result);
                soapResponse = string.Concat(newstring);

            }
            catch (WebException e) //bad web request error
            {
                soapResponse = e.Message;
            }

            return soapResponse; 


       }
    }
}
