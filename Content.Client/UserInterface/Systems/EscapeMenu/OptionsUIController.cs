using Content.Client.GameBar;
using Content.Client.Gameplay;
using Content.Client.Options.UI;
using Content.Shared.Input;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Console;


namespace Content.Client.UserInterface.Systems.EscapeMenu;


[UsedImplicitly]
public sealed class OptionsUIController : UIController
{
    [Dependency] private readonly IConsoleHost      _con               = default!;
    [Dependency] private readonly GameBarManager _gameBarManager = null!;

    public override void Initialize()
    {
        _con.RegisterCommand(
            "options",
            Loc.GetString("cmd-options-desc"),
            Loc.GetString("cmd-options-help"),
            OptionsCommand);

        _gameBarManager
            .GetCategory(GameBarCategory.Global)
            .RegisterItem(
                new(
                    new("global-menu-global-options-item"),
                    Callback: ToggleWindow,
                    Function: ContentKeyFunctions.OpenOptionsWindow
                )
            );
    }

    private void OptionsCommand(IConsoleShell shell, string argStr, string[] args) => ToggleWindow();

    private OptionsMenu _optionsWindow = default!;

    private void EnsureWindow()
    {
        if (_optionsWindow is { Disposed: false })
            return;

        _optionsWindow = UIManager.CreateWindow<OptionsMenu>();
    }

    public void OpenWindow()
    {
        EnsureWindow();

        _optionsWindow.UpdateTabs();

        _optionsWindow.OpenCentered();
        _optionsWindow.MoveToFront();
    }

    public void ToggleWindow()
    {
        EnsureWindow();

        if (_optionsWindow.IsOpen)
        {
            _optionsWindow.Close();
        }
        else
        {
            OpenWindow();
        }
    }
}
