using Avalonia.Media;

namespace WorldCreator.Model;

public class MenuItemModel
{
    public MenuItemModel(string header, StreamGeometry icon)
    {
        Header = header;
        Icon = icon;
    }

    public MenuItemModel()
    {
    }

    public string? Header { get; set; }
    public StreamGeometry? Icon { get; set; }
}