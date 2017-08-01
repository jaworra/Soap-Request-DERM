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

using System.Windows.Forms;

namespace MapInfoSoap
{
    public class SoapRequests
    {
        // Takes address information and returns records that match the criteria best
        public static string ValidateAddressSoapRequest(string unitNumber, string buildingName, string numberFirst, string numberSuffixFirst, string numberLast, string numberSuffixLast, string roadName, string roadNameType, string roadNameSuffix, string localityName, string localGovernmentName, string stateName)
        {
            var uri = new Uri("https://information.qld.gov.au/service/land/property/ValidationService/1/soap");

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
            // Construct request
            var uri = new Uri("https://information.qld.gov.au/service/land/property/ValidationService/1/soap");

            HttpWebRequest req = (HttpWebRequest)WebRequest.CreateDefault(uri);
            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Method = "POST";
            req.Accept = "text/xml";
            req.Headers.Add("SOAPAction", @"information.derm.qld.gov.au/service/Addressing/ValidationService/1/soap/IValidationService/ParseValidAddress");

            // Create XML body for the request
            XmlDocument soapEnv = new XmlDocument();
            soapEnv.LoadXml(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:soap='information.derm.qld.gov.au/service/Addressing/ValidationService/1/soap'><soapenv:Header/><soapenv:Body><soap:ParseValidAddress><soap:addressString>" + address + "</soap:addressString></soap:ParseValidAddress></soapenv:Body></soapenv:Envelope>");

            using (Stream stream = req.GetRequestStream())
            {
                soapEnv.Save(stream);
            }

            // Begin the request
            IAsyncResult asyncRes = req.BeginGetResponse(null, null);

            // Wait for the response
            asyncRes.AsyncWaitHandle.WaitOne();

            // Read the response
            string soapResponse;

            try
            {
                using (WebResponse webResponse = req.EndGetResponse(asyncRes))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResponse = rd.ReadToEnd();
                    }
                }

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
            catch(WebException e) //bad web request error
            {
                soapResponse = e.Message;
            }
            catch(Exception e) //bad request error
            {
                soapResponse = e.Message;
            }

            return soapResponse;

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
