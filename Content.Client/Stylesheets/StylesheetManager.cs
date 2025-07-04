using Content.Client.UIKit;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;


namespace Content.Client.Stylesheets;


public sealed class StylesheetManager : IStylesheetManager
{
    [Dependency] private readonly IUserInterfaceManager _userInterfaceManager = null!;
    [Dependency] private readonly IResourceCache        _resourceCache        = null!;
    [Dependency] private readonly TypographyManager     _typographyManager    = null!;

    public Stylesheet SheetNano { get; private set; } = null!;

    public void Initialize()
    {
        SheetNano = new StyleNano(_resourceCache, _typographyManager).Stylesheet;

        _userInterfaceManager.Stylesheet = SheetNano;
    }
}
