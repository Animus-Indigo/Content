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

using Content.Server.Atmos.Piping.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos;
using Robust.Shared.Map;


namespace Content.Server.Atmos.Piping.EntitySystems;


public sealed class AtmosPipeSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = null!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AtmosPipeComponent, NodeGroupsRebuilt>(OnNodeUpdate);
    }

    private void OnNodeUpdate(EntityUid uid, AtmosPipeComponent component, ref NodeGroupsRebuilt args) =>
        UpdateAppearance(args.NodeOwner, component);

    private void UpdateAppearance(EntityUid uid, AtmosPipeComponent? component)
    {
        if (!Resolve(uid, ref component, false))
            return;

        if (!TryComp(uid, out AppearanceComponent? appearance))
            return;

        if (!TryComp(uid, out NodeContainerComponent? container))
            return;

        UpdateConnectedVisuals(uid, component, appearance, container);
    }

    private void UpdateConnectedVisuals(
        EntityUid uid,
        AtmosPipeComponent? component,
        AppearanceComponent? appearance,
        NodeContainerComponent? container
    )
    {
        if (!Resolve(uid, ref component))
            return;

        if (!Resolve(uid, ref container))
            return;

        var anyPipeNodes = false;

        foreach (var node in container.Nodes.Values)
        {
            if (node is not PipeNode)
                continue;

            foreach (var connectedNode in node.ReachableNodes)
            {
                if (connectedNode is not PipeNode || connectedNode.Owner == uid)
                    continue;

                anyPipeNodes = true;
                break;
            }
        }

        _appearance.SetData(uid, PipeVisuals.Connected, anyPipeNodes, appearance);
    }
}
