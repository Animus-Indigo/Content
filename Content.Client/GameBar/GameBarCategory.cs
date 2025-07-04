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

namespace Content.Client.GameBar;


public static class GameBarCategory
{
    public static readonly GameBarCategoryDef Global = new(
        new("global-menu-global-category"),
        GameBarCategoryPriority.Global,
        IsIcon: true
    );

    public static readonly GameBarCategoryDef Character = new(
        new("global-menu-character-category"),
        GameBarCategoryPriority.Character
    );

    public static readonly GameBarCategoryDef Ghost = new(
        new("global-menu-ghost-category"),
        GameBarCategoryPriority.Ghost
    );

    public static readonly GameBarCategoryDef Admin = new(
        new("global-menu-admin-category"),
        GameBarCategoryPriority.Admin
    );

    public static readonly GameBarCategoryDef Sandbox = new(
        new("global-menu-sandbox-category"),
        GameBarCategoryPriority.Sandbox
    );
}
