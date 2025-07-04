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

using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Utility;


namespace Content.Client.UIKit;


public sealed class TypographyManager
{
    [Dependency] private readonly IResourceCache _cache = null!;

    private readonly List<string> _symbolFontPaths =
        new()
        {
            "/Fonts/NotoSansSymbols/NotoSansSymbols-Regular.ttf", "/Fonts/NotoSansSymbols/NotoSansSymbols2-Regular.ttf"
        };

    public VectorFont GetSymbolsFont(
        bool       filled,
        TextStyle  style  = TextStyle.Body,
        FontWeight weight = FontWeight.Regular
    )
    {
        if (weight > FontWeight.Bold)
            weight = FontWeight.Bold;

        var path =
            $"/Fonts/MaterialSymbols/MaterialSymbols-{(filled ? "Filled-" : "")}{weight.ToPostfix()}.otf";

        var size         = style.ToPx();
        var fontResource = _cache.GetResource<FontResource>(path);

        return new(fontResource, size);
    }

    public StackedFont GetFont(
        FontType     type,
        TextStyle    style    = TextStyle.Body,
        FontWeight   weight   = FontWeight.Regular,
        FontModifier modifier = FontModifier.Normal
    ) =>
        GetFont(type, style.ToPx(), weight, modifier);

    public StackedFont GetFont(
        FontType     type,
        int          customSize,
        FontWeight   weight   = FontWeight.Regular,
        FontModifier modifier = FontModifier.Normal
    )
    {
        var basePath = type.ToBasePath();

        weight = type switch
        {
            FontType.Serif => weight switch
            {
                < FontWeight.Light => FontWeight.Light,
                _                  => weight
            },
            FontType.Mono => weight switch
            {
                < FontWeight.Regular => FontWeight.Regular,
                FontWeight.Medium    => FontWeight.Regular,
                FontWeight.SemiBold  => FontWeight.Bold,
                > FontWeight.Bold    => FontWeight.Bold,
                _                    => weight
            },
            _ => weight
        };

        var weightPostfix   = weight.ToPostfix();
        var modifierPostfix = modifier.ToPostfix();

        var path         = new ResPath($"{basePath}-{weightPostfix}{modifierPostfix}.otf");
        var fontResource = _cache.GetResource<FontResource>(path);

        var fonts = new Font[_symbolFontPaths.Count + 1];

        fonts[0] = new VectorFont(fontResource, customSize);

        for (var i = 1; i < _symbolFontPaths.Count + 1; i++)
        {
            var symbolFontResource = _cache.GetResource<FontResource>(_symbolFontPaths[i - 1]);

            fonts[i] = new VectorFont(symbolFontResource, customSize);
        }

        return new(fonts);
    }
}
