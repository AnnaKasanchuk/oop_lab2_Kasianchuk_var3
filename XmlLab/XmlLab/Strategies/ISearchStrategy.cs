using XmlLab.Models;

namespace XmlLab.Strategies;

public interface ISearchStrategy
{
    string Name { get; }
    List<ResidentInfo> Search(string filePath, IReadOnlyDictionary<string, string> criteria);
}