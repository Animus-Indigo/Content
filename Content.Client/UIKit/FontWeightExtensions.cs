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


public static class FontWeightExtensions
{
    public static string ToPostfix(this FontWeight weight) =>
        weight switch
        {
            FontWeight.Thin       => "Thin",
            FontWeight.ExtraLight => "ExtraLight",
            FontWeight.Light      => "Light",
            FontWeight.Regular    => "Regular",
            FontWeight.Medium     => "Medium",
            FontWeight.SemiBold   => "SemiBold",
            FontWeight.Bold       => "Bold",
            FontWeight.ExtraBold  => "ExtraBold",
            FontWeight.Black      => "Black",
            _                     => throw new ArgumentOutOfRangeException(nameof(weight), weight, null)
        };
}
