namespace XmlLab.Services;

public static class ExitGuard
{
    public static bool Confirmed { get; set; }

    public static Task<bool> AskAsync() =>
        Application.Current!.Windows[0].Page!.DisplayAlert(
            "Вихід", "Чи дійсно ви хочете завершити роботу з програмою?", "Так", "Ні");
}