using Content.Client.GameBar;
using Content.Client.Language;
using Content.Client.Gameplay;
using Content.Shared.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;
using JetBrains.Annotations;


namespace Content.Client.UserInterface.Systems.Language;


[UsedImplicitly]
public sealed class LanguageMenuUIController : UIController, IOnStateEntered<GameplayState>,
    IOnStateExited<GameplayState>
{
    [Dependency] private readonly GameBarManager _gameBarManager = null!;

    public LanguageMenuWindow? LanguageWindow;

    public override void Initialize()
    {
        base.Initialize();

        _gameBarManager
            .GetCategory(GameBarCategory.Character)
            .RegisterItem(
                new(
                    new("global-menu-character-language-window-item"),
                    Callback: ToggleWindow,
                    Function: ContentKeyFunctions.OpenLanguageMenu,
                    InGameState: typeof(GameplayState)
                )
            );
    }

    public void OnStateEntered(GameplayState state)
    {
        DebugTools.Assert(LanguageWindow == null);

        LanguageWindow = UIManager.CreateWindow<LanguageMenuWindow>();
        LayoutContainer.SetAnchorPreset(LanguageWindow, LayoutContainer.LayoutPreset.CenterTop);
    }

    public void OnStateExited(GameplayState state)
    {
        if (LanguageWindow != null)
        {
            LanguageWindow.Dispose();
            LanguageWindow = null;
        }
    }

    private void ToggleWindow()
    {
        if (LanguageWindow == null)
            return;

        if (LanguageWindow.IsOpen)
            LanguageWindow.Close();
        else
            LanguageWindow.Open();
    }
}
