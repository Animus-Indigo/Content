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

namespace Content.Client.UIKit;


public readonly record struct Rounding(float TopLeft, float TopRight, float BottomRight, float BottomLeft)
{
    public const float Large = 18.0f;

    public const float Medium = 12.0f;

    public const float Small = 8.0f;

    public const float Xs = 6.0f;

    public Rounding(float uniform) : this(uniform, uniform, uniform, uniform) { }

    public float Sum() => TopLeft + TopRight + BottomRight + BottomLeft;
}
