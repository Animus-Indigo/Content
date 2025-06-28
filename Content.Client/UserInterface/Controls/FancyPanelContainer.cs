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

using Content.Shared.InterfaceGuidelines;
using Robust.Client.UserInterface.Controls;


namespace Content.Client.UserInterface.Controls;


public sealed class FancyPanelContainer : PanelContainer
{
    public Rounding Rounding { get; set; } = new(0.0f);

    public FancyPanelContainer()
    {
        PanelOverride = new RectBox
        {
            BackgroundColor = Colors.IndigoGray1400,
            Borders         = new(Colors.WindowInsetBorder, new(2.0f)),
            Rounding        = Rounding
        };
    }
}
