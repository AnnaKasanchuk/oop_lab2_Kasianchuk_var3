using System.Xml.Linq;
using XmlLab.Models;

namespace XmlLab.Strategies;

public class LinqSearchStrategy : ISearchStrategy
{
    public string Name => "LINQ to XML";

    public List<ResidentInfo> Search(string filePath, IReadOnlyDictionary<string, string> criteria)
    {
        var doc = XDocument.Load(filePath);

        // динамічний предикат, який будується з обраних критеріїв
        Func<XElement, bool> predicate = e =>
            criteria.All(c => (string?)e.Attribute(c.Key) == c.Value);

        return doc.Descendants("Resident")
            .Where(predicate)
            .Select(e =>
            {
                var resident = new ResidentInfo();
                foreach (var a in e.Attributes())
                    resident.Attributes[a.Name.LocalName] = a.Value;

                resident.Residences.AddRange(e.Elements("Residence").Select(r => new ResidenceInfo
                {
                    Building = (string?)r.Attribute("Building") ?? "",
                    Room = (string?)r.Attribute("Room") ?? "",
                    CheckIn = (string?)r.Attribute("CheckIn") ?? "",
                    CheckOut = (string?)r.Attribute("CheckOut") ?? ""
                }));
                return resident;
            })
            .ToList();
    }
}