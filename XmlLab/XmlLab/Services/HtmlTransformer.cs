using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using XmlLab.Models;

namespace XmlLab.Services;

public static class HtmlTransformer
{
    public static async Task<string> TransformAsync(IEnumerable<ResidentInfo> residents)
    {
        XDocument source = BuildXml(residents);

        using var xslStream = await FileSystem.OpenAppPackageFileAsync("Hostel.xsl");
        using var xslReader = XmlReader.Create(xslStream);
        var xslt = new XslCompiledTransform();
        xslt.Load(xslReader);

        using var input = source.CreateReader();
        using var writer = new StringWriter();
        xslt.Transform(input, null, writer);
        return writer.ToString();
    }

    private static XDocument BuildXml(IEnumerable<ResidentInfo> residents) =>
        new(new XElement("Hostel",
            residents.Select(r => new XElement("Resident",
                r.Attributes.Select(a => new XAttribute(a.Key, a.Value)),
                r.Residences.Select(s => new XElement("Residence",
                    new XAttribute("Building", s.Building),
                    new XAttribute("Room", s.Room),
                    new XAttribute("CheckIn", s.CheckIn),
                    new XAttribute("CheckOut", s.CheckOut)))))));
}