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

using Content.Client.UIKit;
using Content.Client.UserInterface;
using Robust.Client.Graphics;


namespace Content.Client.Graphics;


public static class DrawingHandleScreenExtensions
{
    public static void DrawRoundedRect(
        this DrawingHandleScreen handle,
        UIBox2                   box,
        Rounding                 rounding,
        Color                    color,
        float                    uiScale,
        bool                     filled = true
    )
    {
        if (rounding.Sum() <= 0.0f)
        {
            handle.DrawRect(box, color, filled);

            return;
        }

        // Circle vertices use Color.FromSrgb, WTF?
        var srgbBackgroundColor = Color.ToSrgb(color);

        // Corners

        var topLeftRadius     = rounding.TopLeft * uiScale;
        var topRightRadius    = rounding.TopRight * uiScale;
        var bottomRightRadius = rounding.BottomRight * uiScale;
        var bottomLeftRadius  = rounding.BottomLeft * uiScale;

        var topLeftCorner = box.TopLeft with
        {
            X = box.TopLeft.X + topLeftRadius, Y = box.TopLeft.Y + topLeftRadius
        };
        var topRightCorner = box.TopRight with
        {
            X = box.TopRight.X - topRightRadius, Y = box.TopRight.Y + topRightRadius
        };
        var bottomRightCorner = box.BottomRight with
        {
            X = box.BottomRight.X - bottomRightRadius, Y = box.BottomRight.Y - bottomRightRadius
        };
        var bottomLeftCorner = box.BottomLeft with
        {
            X = box.BottomLeft.X + bottomLeftRadius, Y = box.BottomLeft.Y - bottomLeftRadius
        };

        if (topLeftRadius != 0.0f)
        {
            handle.DrawCircle(
                topLeftCorner,
                topLeftRadius,
                srgbBackgroundColor,
                filled
            );
        }

        if (topRightRadius != 0.0f)
        {
            handle.DrawCircle(
                topRightCorner,
                topRightRadius,
                srgbBackgroundColor,
                filled
            );
        }

        if (bottomRightRadius != 0.0f)
        {
            handle.DrawCircle(
                bottomRightCorner,
                bottomRightRadius,
                srgbBackgroundColor,
                filled
            );
        }

        if (bottomLeftRadius != 0.0f)
        {
            handle.DrawCircle(
                bottomLeftCorner,
                bottomLeftRadius,
                srgbBackgroundColor,
                filled
            );
        }

        // Borders

        var maxTopRadius    = Math.Max(topLeftRadius, topRightRadius);
        var maxRightRadius  = Math.Max(topRightRadius, bottomRightRadius);
        var maxBottomRadius = Math.Max(bottomLeftRadius, bottomRightRadius);
        var maxLeftRadius   = Math.Max(topLeftRadius, bottomLeftRadius);

        var topBorderRect = UIBox2.FromDimensions(
            box.Left + topLeftRadius,
            box.Top,
            box.Width - topLeftRadius - topRightRadius,
            maxTopRadius
        );
        var rightBorderRect = UIBox2.FromDimensions(
            box.Right - maxRightRadius,
            box.Top + topRightRadius,
            maxRightRadius,
            box.Height - topRightRadius - bottomRightRadius
        );
        var bottomBorderRect = UIBox2.FromDimensions(
            box.Left + bottomLeftRadius,
            box.Bottom - maxBottomRadius,
            box.Width - bottomLeftRadius - bottomRightRadius,
            maxBottomRadius
        );
        var leftBorderRect = UIBox2.FromDimensions(
            box.Left,
            box.Top + topLeftRadius,
            maxLeftRadius,
            box.Height - topLeftRadius - bottomLeftRadius
        );

        handle.DrawRect(topBorderRect, color, filled);
        handle.DrawRect(rightBorderRect, color, filled);
        handle.DrawRect(bottomBorderRect, color, filled);
        handle.DrawRect(leftBorderRect, color, filled);

        // Inner rect

        var innerRect = new UIBox2(topBorderRect.BottomLeft, rightBorderRect.BottomLeft);

        handle.DrawRect(innerRect, color, filled);
    }
}
