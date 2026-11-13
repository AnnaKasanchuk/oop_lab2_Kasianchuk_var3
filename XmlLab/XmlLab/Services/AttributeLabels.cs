namespace XmlLab.Services;

public static class AttributeLabels
{
    private static readonly Dictionary<string, string> Labels = new()
    {
        ["Name"] = "ПІП",
        ["Faculty"] = "Факультет",
        ["Department"] = "Кафедра",
        ["Course"] = "Курс",
        ["Phone"] = "Телефон"
    };

    public static string ToUa(string name) =>
        Labels.TryGetValue(name, out var label) ? label : name;

    public static string FromUa(string label) =>
        Labels.FirstOrDefault(p => p.Value == label).Key ?? label;
}