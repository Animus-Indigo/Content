// Copyright (C) 2025 Igor Spichkin

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Shared.Localizations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Timing;
using Robust.Shared.Utility;


namespace Content.Client.GameBar;


public sealed partial class GameBarUIController : UIController, IOnStateChanged<GameplayState>,
    IOnStateChanged<LobbyState>
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = null!;

    public event Action<LocalizedString, LocalizedString>? ItemPressed;

    private UI.GameBar? GlobalMenu => UIManager.GetActiveUIWidgetOrNull<UI.GameBar>();

    private IReadOnlyList<UI.GameBar.Category> _categories = [];
    private bool                               _isDirty;

    public void Populate(IReadOnlyList<UI.GameBar.Category> categories)
    {
        _categories = categories;
        _isDirty    = true;
    }

    public override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (!_isDirty)
            return;

        _isDirty = false;
        ReBuild();
    }

    private void ReBuild() => GlobalMenu?.Populate(_categories);

    private void OnItemPressed(LocalizedString category, LocalizedString item) => ItemPressed?.Invoke(category, item);

    public void OnStateEntered(GameplayState state) => OnGlobalMenuShow();

    public void OnStateExited(GameplayState state) => OnGlobalMenuHide();

    public void OnStateEntered(LobbyState state) => OnGlobalMenuShow();

    public void OnStateExited(LobbyState state) => OnGlobalMenuHide();

    private void OnGlobalMenuShow()
    {
        DebugTools.AssertNotNull(GlobalMenu);

        GlobalMenu!.ItemPressed += OnItemPressed;
        _uiManager.DeferAction(() =>
        {
            GlobalMenu?.ForceRunStyleUpdate();
        });
    }

    private void OnGlobalMenuHide()
    {
        if (GlobalMenu is { } globalMenu)
            globalMenu.ItemPressed -= OnItemPressed;
    }
}
