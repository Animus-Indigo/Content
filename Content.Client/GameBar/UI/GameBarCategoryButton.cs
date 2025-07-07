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

using Content.Shared.UIKit;
using Robust.Client.UserInterface.Controls;


namespace Content.Client.GameBar.UI;


public sealed class GameBarCategoryButton : Button
{
    public GameBarCategoryButton(bool isIcon)
    {
        SetOnlyStyleClass(UIStyleClasses.GlobalMenuCategoryButton);
        Label.SetOnlyStyleClass(isIcon ? UIStyleClasses.GlobalMenuCategoryIcon : UIStyleClasses.GlobalMenuCategoryLabel);
        Label.Margin = new(16.0f, 0.0f);

        if (isIcon)
            Label.VAlign = Label.VAlignMode.Center;
    }
}
