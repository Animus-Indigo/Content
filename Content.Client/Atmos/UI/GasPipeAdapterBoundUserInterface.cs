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

using Content.Shared.Atmos.Piping.Binary.GasPipeAdapter;


namespace Content.Client.Atmos.UI;


public sealed class GasPipeAdapterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private GasPipeAdapterWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window?.Close();

        _window = new();
        _window.OpenCentered();
        _window.OnClose += Close;
        _window.OnInletLayerSelected += OnInletLayerSelected;
        _window.OnOutletLayerSelected += OnOutletLayerSelected;
    }

    private void OnOutletLayerSelected(int layer) =>
        SendMessage(new GasPipeAdapterLayerSelectedMessage(GasPipeAdapterLayerType.Outlet, layer));

    private void OnInletLayerSelected(int layer) =>
        SendMessage(new GasPipeAdapterLayerSelectedMessage(GasPipeAdapterLayerType.Inlet, layer));

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not GasPipeAdapterUiState castedState)
            return;

        _window?.UpdateState(castedState);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
            return;

        _window?.Close();
    }
}
