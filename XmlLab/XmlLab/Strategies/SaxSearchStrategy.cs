using System.Xml;
using XmlLab.Models;

namespace XmlLab.Strategies;

public class SaxSearchStrategy : ISearchStrategy
{
    public string Name => "SAX (XmlReader)";

    public List<ResidentInfo> Search(string filePath, IReadOnlyDictionary<string, string> criteria)
    {
        var result = new List<ResidentInfo>();
        ResidentInfo? current = null;

        using var reader = XmlReader.Create(filePath);
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == "Resident")
                {
                    current = new ResidentInfo();
                    while (reader.MoveToNextAttribute())
                        current.Attributes[reader.Name] = reader.Value;
                    reader.MoveToElement();

                    if (reader.IsEmptyElement)
                    {
                        TryAdd(result, current, criteria);
                        current = null;
                    }
                }
                else if (reader.Name == "Residence" && current != null)
                {
                    current.Residences.Add(new ResidenceInfo
                    {
                        Building = reader.GetAttribute("Building") ?? "",
                        Room = reader.GetAttribute("Room") ?? "",
                        CheckIn = reader.GetAttribute("CheckIn") ?? "",
                        CheckOut = reader.GetAttribute("CheckOut") ?? ""
                    });
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Resident")
            {
                TryAdd(result, current, criteria);
                current = null;
            }
        }
        return result;
    }

    private static void TryAdd(List<ResidentInfo> result, ResidentInfo? resident,
        IReadOnlyDictionary<string, string> criteria)
    {
        if (resident != null &&
            criteria.All(c => resident.Attributes.TryGetValue(c.Key, out var v) && v == c.Value))
            result.Add(resident);
    }
}