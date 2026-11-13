using System.Xml;
using XmlLab.Models;

namespace XmlLab.Strategies;

public class DomSearchStrategy : ISearchStrategy
{
    public string Name => "DOM (XmlDocument + XPath)";

    public List<ResidentInfo> Search(string filePath, IReadOnlyDictionary<string, string> criteria)
    {
        var doc = new XmlDocument();
        doc.Load(filePath);

        // динамічна генерація запиту: //Resident[@Faculty='ФІТ' and @Course='2']
        string conditions = string.Join(" and ", criteria.Select(c => $"@{c.Key}={Literal(c.Value)}"));
        string xpath = criteria.Count > 0 ? $"//Resident[{conditions}]" : "//Resident";

        var result = new List<ResidentInfo>();
        foreach (XmlNode node in doc.SelectNodes(xpath)!)
        {
            var resident = new ResidentInfo();
            foreach (XmlAttribute attr in node.Attributes!)
                resident.Attributes[attr.Name] = attr.Value;

            foreach (XmlNode child in node.SelectNodes("Residence")!)
            {
                resident.Residences.Add(new ResidenceInfo
                {
                    Building = child.Attributes?["Building"]?.Value ?? "",
                    Room = child.Attributes?["Room"]?.Value ?? "",
                    CheckIn = child.Attributes?["CheckIn"]?.Value ?? "",
                    CheckOut = child.Attributes?["CheckOut"]?.Value ?? ""
                });
            }
            result.Add(resident);
        }
        return result;
    }

    private static string Literal(string value) =>
        value.Contains('\'') ? $"\"{value}\"" : $"'{value}'";
}