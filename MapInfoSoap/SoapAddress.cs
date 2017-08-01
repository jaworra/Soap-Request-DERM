using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace MapInfoSoap
{
    class SoapAddress
    {
        public int Number { get; set; }
        public string RoadType { get; set; }
        public string RoadName { get; set; }
        public string AdminArea { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public SoapAddress(XElement SourceElemt)
        {            
            XElement lA = SourceElemt.Elements().First();
            XNamespace lANS = lA.GetDefaultNamespace();
            Number = Convert.ToInt32(lA.Attribute("numberFirst").Value);
            RoadType = lA.Element(lANS + "RoadName").Attribute("roadNameType").Value;
            RoadName = lA.Element(lANS + "RoadName").Attribute("roadName").Value;
            IEnumerable<XElement> aAEs = (from e in lA.Descendants() where e.Name.LocalName == "AdministrativeArea" select e);
            XElement aAE = aAEs.First();
            AdminArea = aAE.Attribute("adminAreaName").Value;
            //lA.Descendants(lANS + "AdministrativeArea");
            //AdminArea = ;
            Longitude = SourceElemt.Element(lANS + "Center").Attribute("longitude").Value;
            Latitude = SourceElemt.Element(lANS + "Center").Attribute("latitude").Value;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}, {3}|{4}|{5};", Number, RoadName, RoadType, AdminArea, Latitude, Longitude);
        }
    }
}
