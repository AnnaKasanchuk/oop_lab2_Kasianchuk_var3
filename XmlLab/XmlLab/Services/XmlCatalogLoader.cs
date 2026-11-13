using System.Xml.Linq;

namespace XmlLab.Services;

public static class XmlCatalogLoader
{
    public static Dictionary<string, SortedSet<string>> Load(string filePath)
    {
        var catalog = new Dictionary<string, SortedSet<string>>();
        foreach (var resident in XDocument.Load(filePath).Descendants("Resident"))
        {
            foreach (var attr in resident.Attributes())
            {
                string name = attr.Name.LocalName;
                if (!catalog.TryGetValue(name, out var values))
                    catalog[name] = values = new SortedSet<string>();
                values.Add(attr.Value);
            }
        }
        return catalog;
    }
}