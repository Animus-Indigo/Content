using Content.Client.Construction.UI;
using Content.Client.GameBar;
using Content.Client.Gameplay;
using Content.Shared.Input;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Crafting;

[UsedImplicitly]
public sealed class CraftingUIController : UIController, IOnStateChanged<GameplayState>
{
    [Dependency] private readonly GameBarManager _gameBarManager = null!;

    private ConstructionMenuPresenter? _presenter;

    public override void Initialize()
    {
        base.Initialize();

        _gameBarManager
            .GetCategory(GameBarCategory.Character)
            .RegisterItem(
                new(
                    new("global-menu-character-crafting-window-item"),
                    Callback: () => _presenter?.ToggleWindow(),
                    Function: ContentKeyFunctions.OpenCraftingMenu,
                    InGameState: typeof(GameplayState)
                )
            );
    }

    public void OnStateEntered(GameplayState state)
    {
        DebugTools.Assert(_presenter == null);
        _presenter = new ConstructionMenuPresenter();
    }

    public void OnStateExited(GameplayState state)
    {
        if (_presenter == null)
            return;
        UnloadButton(_presenter);
        _presenter.Dispose();
        _presenter = null;
    }

    internal void UnloadButton(ConstructionMenuPresenter? presenter = null)
    {
        if (presenter == null)
        {
            presenter ??= _presenter;
            if (presenter == null)
            {
                return;
            }
        }
    }
}
