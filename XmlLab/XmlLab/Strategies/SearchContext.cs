using XmlLab.Models;

namespace XmlLab.Strategies;

public class SearchContext
{
    private ISearchStrategy? _strategy;

    public void SetStrategy(ISearchStrategy strategy) => _strategy = strategy;

    public List<ResidentInfo> Execute(string filePath, IReadOnlyDictionary<string, string> criteria)
    {
        if (_strategy == null)
            throw new InvalidOperationException("Стратегію не обрано");
        return _strategy.Search(filePath, criteria);
    }
}