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

using Content.Client.CombatMode;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Utility;


namespace Content.Client.GameBar;


public sealed partial class GameBarUIController : IOnSystemChanged<CombatModeSystem>
{
    public void OnSystemLoaded(CombatModeSystem system) =>
        system.LocalPlayerCombatModeUpdated += OnLocalPlayerCombatModeUpdated;

    public void OnSystemUnloaded(CombatModeSystem system) =>
        system.LocalPlayerCombatModeUpdated -= OnLocalPlayerCombatModeUpdated;

    private void OnLocalPlayerCombatModeUpdated(bool state)
    {
        DebugTools.AssertNotNull(GlobalMenu);

        GlobalMenu!.SetStyle(state ? GameBarStyle.Combat : GameBarStyle.Default);
    }
}
