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

using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Piping.Binary.GasPipeAdapter;
using Robust.Server.GameObjects;


namespace Content.Server.Atmos.Piping.Binary.EntitySystems;


public sealed class GasPipeAdapterSystem : EntitySystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = null!;
    [Dependency] private readonly AppearanceSystem _appearanceSystem = null!;
    [Dependency] private readonly NodeContainerSystem _nodeContainerSystem = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GasPipeAdapterComponent, BoundUIOpenedEvent>(OnBoundUiOpened);
        SubscribeLocalEvent<GasPipeAdapterComponent, GasPipeAdapterLayerSelectedMessage>(OnGasPipeAdapterLayerSelected);
    }

    private void OnGasPipeAdapterLayerSelected(
        EntityUid uid,
        GasPipeAdapterComponent component,
        GasPipeAdapterLayerSelectedMessage args
    )
    {
        var layer = Math.Clamp(args.Layer, 0, PipeNode.MaxLayers - 1);

        switch (args.LayerType)
        {
            case GasPipeAdapterLayerType.Inlet:
                component.InletLayer = layer;

                break;
            case GasPipeAdapterLayerType.Outlet:
                component.OutletLayer = layer;

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        UpdateState(uid, component);
    }

    private void UpdateState(EntityUid uid, GasPipeAdapterComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        var nodeContainer = Comp<NodeContainerComponent>(uid);

        if (_nodeContainerSystem.TryGetNode(nodeContainer, component.InletNode, out PipeNode? inletNode))
        {
            inletNode.Layer = component.InletLayer;
            inletNode.ConnectionsEnabled = true;
        }

        if (_nodeContainerSystem.TryGetNode(nodeContainer, component.OutletNode, out PipeNode? outletNode))
        {
            outletNode.Layer = component.OutletLayer;
            outletNode.ConnectionsEnabled = true;
        }

        UpdateAppearance(uid, component);
        UpdateUiState(uid, component);
    }

    private void OnBoundUiOpened(EntityUid uid, GasPipeAdapterComponent component, BoundUIOpenedEvent args)
    {
        UpdateUiState(uid, component);
    }

    private void UpdateAppearance(EntityUid uid, GasPipeAdapterComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        if (!TryComp(uid, out AppearanceComponent? appearance))
            return;

        _appearanceSystem.SetData(uid, GasPipeAdapterVisuals.InletLayer, component.InletLayer, appearance);
        _appearanceSystem.SetData(uid, GasPipeAdapterVisuals.OutletLayer, component.OutletLayer, appearance);
    }

    private void UpdateUiState(EntityUid uid, GasPipeAdapterComponent? component)
    {
        if (!Resolve(uid, ref component))
            return;

        var state = new GasPipeAdapterUiState(component.InletLayer, component.OutletLayer);

        _uiSystem.SetUiState(uid, GasPipeAdapterUiKey.Key, state);
    }
}
