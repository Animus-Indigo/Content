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

using System.Numerics;
using Content.Client.Atmos.Components;
using Content.Shared.Atmos;
using JetBrains.Annotations;
using Robust.Client.GameObjects;


namespace Content.Client.Atmos.Visualizers;


[UsedImplicitly]
public sealed class AtmosPipeConnectionVisualizerSystem : VisualizerSystem<AtmosPipeConnectionVisualsComponent>
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = null!;

    protected override void OnAppearanceChange(
        EntityUid uid,
        AtmosPipeConnectionVisualsComponent component,
        ref AppearanceChangeEvent args
    )
    {
        if (args.Sprite == null)
            return;

        if (!args.Sprite.Visible)
        {
            // This entity is probably below a floor and is not even visible to the user -> don't bother updating sprite data.
            // Note that if the subfloor visuals change, then another AppearanceChangeEvent will get triggered.
            return;
        }

        if (!_appearance.TryGetData<bool>(uid, PipeVisuals.Connected, out _, args.Component))
            _appearance.SetData(uid, PipeVisuals.Connected, false, args.Component);

        UpdateConnectedVisuals(uid, component, ref args);
    }

    private void UpdateConnectedVisuals(
        EntityUid uid,
        AtmosPipeConnectionVisualsComponent _,
        ref AppearanceChangeEvent args
    )
    {
        _appearance.TryGetData<bool>(uid, PipeVisuals.Connected, out var isConnected, args.Component);

        var newScale = isConnected ? new(1f, 1f) : new Vector2(0.8f, 0.8f);

        foreach (var layer in args.Sprite!.AllLayers)
            layer.Scale = newScale;
    }
}

public enum PipeVisualLayers : byte
{
    Pipe
}
