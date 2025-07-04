using Content.Client.Chat.UI;
using Content.Client.GameBar;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Shared.Chat;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Input;
using JetBrains.Annotations;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input.Binding;
using Robust.Shared.Prototypes;


namespace Content.Client.UserInterface.Systems.Emotes;


[UsedImplicitly]
public sealed class EmotesUIController : UIController
{
    [Dependency] private readonly IEntityManager    _entityManager     = default!;
    [Dependency] private readonly IClyde            _displayManager    = default!;
    [Dependency] private readonly IInputManager     _inputManager      = default!;
    [Dependency] private readonly GameBarManager _gameBarManager = null!;

    private EmotesMenu? _menu;

    public override void Initialize()
    {
        base.Initialize();

        _gameBarManager
            .GetCategory(GameBarCategory.Character)
            .RegisterItem(
                new(
                    new("global-menu-character-emote-window-item"),
                    Callback: () => ToggleEmotesMenu(true),
                    Function: ContentKeyFunctions.OpenEmotesMenu,
                    InGameState: typeof(GameplayState)
                )
            );
    }

    private void ToggleEmotesMenu(bool centered)
    {
        if (_menu == null)
        {
            // setup window
            _menu             =  UIManager.CreateWindow<EmotesMenu>();
            _menu.OnClose     += OnWindowClosed;
            _menu.OnPlayEmote += OnPlayEmote;

            if (centered)
            {
                _menu.OpenCentered();
            }
            else
            {
                // Open the menu, centered on the mouse
                var vpSize = _displayManager.ScreenSize;
                _menu.OpenCenteredAt(_inputManager.MouseScreenPosition.Position / vpSize);
            }
        }
        else
        {
            _menu.OnClose     -= OnWindowClosed;
            _menu.OnPlayEmote -= OnPlayEmote;

            CloseMenu();
        }
    }

    private void OnWindowClosed()
    {
        CloseMenu();
    }

    private void CloseMenu()
    {
        if (_menu == null)
            return;

        _menu.Dispose();
        _menu = null;
    }

    private void OnPlayEmote(ProtoId<EmotePrototype> protoId)
    {
        _entityManager.RaisePredictiveEvent(new PlayEmoteMessage(protoId));
    }
}
