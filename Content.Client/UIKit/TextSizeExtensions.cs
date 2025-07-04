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


public static class TextSizeExtensions
{
    public static int ToPx(this TextStyle style) =>
        style switch
        {
            TextStyle.LargeTitle  => 26,
            TextStyle.Title1      => 22,
            TextStyle.Title2      => 17,
            TextStyle.Title3      => 15,
            TextStyle.Headline    => 13,
            TextStyle.Body        => 13,
            TextStyle.Callout     => 12,
            TextStyle.Subheadline => 11,
            TextStyle.Footnote    => 10,
            _                     => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };
}
