namespace XmlLab.Models;

public class ResidenceInfo
{
    public string Building { get; set; } = "";
    public string Room { get; set; } = "";
    public string CheckIn { get; set; } = "";
    public string CheckOut { get; set; } = "";
}

public class ResidentInfo
{
    public Dictionary<string, string> Attributes { get; } = new();
    public List<ResidenceInfo> Residences { get; } = new();
}