using Content.Client.GameBar;
using JetBrains.Annotations;
using Robust.Client.Console;
using Robust.Client.UserInterface.Controllers;


namespace Content.Client.UserInterface.Systems.EscapeMenu;


[UsedImplicitly]
public sealed class EscapeUIController : UIController
{
    [Dependency] private readonly IClientConsoleHost _console           = null!;
    [Dependency] private readonly GameBarManager  _gameBarManager = null!;

    public override void Initialize()
    {
        base.Initialize();

        _gameBarManager
            .GetCategory(GameBarCategory.Global)
            .RegisterItem(
                new(
                    new("global-menu-global-disconnect-item"),
                    Callback: () => _console.ExecuteCommand("disconnect"),
                    Priority: -500
                )
            )
            .RegisterItem(
                new(
                    new("global-menu-global-quit-item"),
                    Callback: () => _console.ExecuteCommand("quit"),
                    Priority: -1000
                )
            );
    }
}
