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

using System.Linq;
using Content.Shared.UIKit;
using Robust.Client.UserInterface.Controls;


namespace Content.Client.UIKit.Controls;


public sealed class UIKOptionButton : UIKButton
{
    public int SelectedId  => _items[SelectedIdx].Id;
    public int SelectedIdx { get; private set; }

    public object? SelectedMetadata => _items[SelectedIdx].Metadata;
    public int     ItemCount        => _items.Count;

    public event Action<ItemSelectedEventArgs>? OnItemSelected;

    private readonly UIKOptionPopup _popup;
    private readonly List<ItemData>   _items = [];

    public UIKOptionButton()
    {
        OnPressed += OnInternalPressed;

        _popup             =  new();
        _popup.OnPopupOpen += OnPopupOpened;
        _popup.OnPopupHide += OnPopupHide;

        IsIconAtLeft   = false;
        Icon           = SymbolIcons.ExpandAll;
        IconTextStyle  = UIKit.TextStyle.Title3;
        IconFontWeight = UIKit.FontWeight.SemiBold;
    }

    private void OnPopupOpened() => UserInterfaceManager.ModalRoot.AddChild(_popup);

    private void OnPopupHide() => UserInterfaceManager.ModalRoot.RemoveChild(_popup);

    public void AddItem(string title, int? id = null)
    {
        id ??= _items.Count;

        if (_items.Any(i => i.Id == id))
            throw new ArgumentException("An item with the same ID already exists.");

        var button = new ItemButton(title);
        button.OnPressed += OnItemButtonPressed;

        var item = new ItemData(id.Value, title, false, null, button);

        _items.Add(item);

        if (_items.Count == 1)
            Select(0);

        _popup.PopupContainer.AddChild(item.Button);
    }

    public void Clear()
    {
        if (_popup.Visible)
            TogglePopup();

        _popup.PopupContainer.RemoveAllChildren();

        foreach (var item in _items)
            item.Button.OnPressed -= OnItemButtonPressed;

        _items.Clear();
        SelectedIdx = -1;
    }

    public int GetItemId(int idx) => _items[idx].Id;

    public object? GetItemMetadata(int idx) => _items[idx].Metadata;

    /// <summary>
    /// Select by index rather than id. Throws exception if item with that index
    /// not in this control.
    /// </summary>
    public void Select(int idx)
    {
        if (SelectedIdx >= 0 && SelectedIdx < _items.Count)
            _items[SelectedIdx].Button.SetIsSelected(false);

        var item = _items[idx];

        SelectedIdx = idx;
        Text        = item.Text;
        item.Button.SetIsSelected(true);
    }

    /// <summary>
    /// Select by index rather than id.
    /// </summary>
    /// <returns>false if item with that index not in this control</returns>
    public bool TrySelect(int idx)
    {
        if (idx < 0 || idx >= _items.Count)
            return false;

        Select(idx);

        return true;
    }

    /// throws exception if item with this ID is not in this control
    public void SelectId(int id) => Select(GetIdx(id));

    /// <returns>false if item with id not in this control</returns>
    public bool TrySelectId(int id)
    {
        for (var i = 0; i < _items.Count; i++)
        {
            var item = _items[i];

            if (item.Id != id)
                continue;

            Select(i);

            return true;
        }

        return false;
    }

    public int GetIdx(int id)
    {
        for (var i = 0; i < _items.Count; i++)
        {
            var item = _items[i];

            if (item.Id != id)
                continue;

            return i;
        }

        throw new IndexOutOfRangeException();
    }

    public void SetItemDisabled(int idx, bool disabled)
    {
        var data = _items[idx] = _items[idx] with { Disabled = disabled, };
        data.Button.Disabled = disabled;
    }

    public void SetItemId(int idx, int id)
    {
        if (_items.Any(i => i.Id == id))
            throw new InvalidOperationException("An item with said ID already exists.");

        _items[idx] = _items[idx] with { Id = id, };
    }

    public void SetItemMetadata(int idx, object metadata) => _items[idx] = _items[idx] with { Metadata = metadata, };

    public void SetItemText(int idx, string text)
    {
        var data = _items[idx] = _items[idx] with { Text = text, };

        if (SelectedId == data.Id)
            Text = text;

        data.Button.ItemNameLabel.Text = text;
    }

    private void OnInternalPressed(ButtonEventArgs ev) => TogglePopup();

    private void OnItemButtonPressed(ButtonEventArgs ev)
    {
        ev.Button.Pressed = false;

        TogglePopup();

        foreach (var item in _items)
        {
            if (item.Button != ev.Button)
                continue;

            OnItemSelected?.Invoke(new(item.Id, this));

            return;
        }

        // Not reachable.
        throw new InvalidOperationException();
    }

    private void TogglePopup()
    {
        if (_popup.Visible)
            _popup.Close();
        else
        {
            _popup.Open();

            UserInterfaceManager.DeferAction(() =>
            {
                var topLeft = ButtonLabel.GlobalRect.TopLeft;
                var child   = (ItemButton) _popup.PopupContainer.GetChild(SelectedIdx);

                topLeft.X -= child.ItemNameLabel.GlobalRect.Left - _popup.GlobalRect.Left;
                topLeft.Y -= child.ItemNameLabel.GlobalRect.Top - _popup.GlobalRect.Top;

                var targetPopupRight = topLeft.X + _popup.GlobalRect.Width;

                _popup.MinWidth = Math.Max(
                    GlobalRect.Width,
                    _popup.GlobalRect.Width + (GlobalRect.Right - targetPopupRight));

                _popup.Close();
                _popup.Open(UIBox2.FromDimensions(topLeft, new()));
            });
        }
    }

    protected override void ExitedTree()
    {
        base.ExitedTree();

        if (_popup.Visible)
            TogglePopup();
    }

    public sealed class ItemSelectedEventArgs(int id, UIKOptionButton button) : EventArgs
    {
        public UIKOptionButton Button { get; } = button;

        /// <summary>
        ///     The ID of the item that has been selected.
        /// </summary>
        public int Id { get; } = id;
    }

    private record struct ItemData(int Id, string Text, bool Disabled, object? Metadata, ItemButton Button);

    private sealed class ItemButton : ContainerButton
    {
        public UIKIcon SelectedMarkLabel { get; }
        public Label     ItemNameLabel     { get; }

        public ItemButton(string title)
        {
            ToggleMode = true;

            SetOnlyStyleClass("FancyPopupItemButton");

            var hBox = new BoxContainer
            {
                Orientation        = BoxContainer.LayoutOrientation.Horizontal,
                HorizontalExpand   = true,
                SeparationOverride = 4
            };

            SelectedMarkLabel = new()
            {
                Text                = " ",
                HorizontalAlignment = HAlignment.Left,
                TextStyle           = UIKit.TextStyle.Title3,
                FontWeight          = UIKit.FontWeight.SemiBold
            };

            hBox.AddChild(SelectedMarkLabel);

            ItemNameLabel = new()
            {
                Text                = title,
                HorizontalAlignment = HAlignment.Right
            };

            hBox.AddChild(ItemNameLabel);

            AddChild(hBox);
        }

        public void SetIsSelected(bool state)
        {
            SelectedMarkLabel.Text = state ? SymbolIcons.Check : " ";
            Pressed                = state;
        }
    }
}
