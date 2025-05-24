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

using Content.Client.Atmos.Visualizers.Components;
using Content.Shared.Atmos.Piping.Binary.GasPipeAdapter;
using Robust.Client.GameObjects;


namespace Content.Client.Atmos.Visualizers;


public sealed class GasPipeAdapterVisualizerSystem : VisualizerSystem<GasPipeAdapterVisualsComponent>
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = null!;

    protected override void OnAppearanceChange(
        EntityUid uid,
        GasPipeAdapterVisualsComponent component,
        ref AppearanceChangeEvent args
    )
    {
        base.OnAppearanceChange(uid, component, ref args);

        if (args.Sprite is null || args.Sprite.Visible == false)
            return;

        if (!_appearance.TryGetData(uid, GasPipeAdapterVisuals.InletLayer, out int? inletLayer))
        {
            inletLayer = 2;
            _appearance.SetData(uid, GasPipeAdapterVisuals.InletLayer, inletLayer);
        }

        if (!_appearance.TryGetData(uid, GasPipeAdapterVisuals.OutletLayer, out int? outletLayer))
        {
            outletLayer = 2;
            _appearance.SetData(uid, GasPipeAdapterVisuals.OutletLayer, outletLayer);
        }

        if (args.Sprite.LayerMapTryGet(GasPipeAdapterVisualLayers.InletSelected, out var inletSelectedLayer))
            args.Sprite[inletSelectedLayer].RsiState = $"inlet{component.BaseSelectedLayer}_{inletLayer}";

        if (args.Sprite.LayerMapTryGet(GasPipeAdapterVisualLayers.OutletSelected, out var outletSelectedLayer))
            args.Sprite[outletSelectedLayer].RsiState = $"outlet{component.BaseSelectedLayer}_{outletLayer}";
    }
}

public enum GasPipeAdapterVisualLayers : byte
{
    InletSelected,
    OutletSelected
}
