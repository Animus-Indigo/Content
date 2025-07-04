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
using Content.Client.GameBar.UI;
using Content.Shared.Localizations;
using Robust.Client;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Input.Binding;
using Robust.Shared.Timing;


namespace Content.Client.GameBar;


public sealed class GameBarManager
{
    [Dependency] private readonly IStateManager         _stateManager = null!;
    [Dependency] private readonly IBaseClient           _client       = null!;
    [Dependency] private readonly IUserInterfaceManager _uiManager    = null!;
    [Dependency] private readonly IInputManager         _inputManager = null!;

    private GameBarUIController                     _uiController         = null!;
    private Dictionary<LocalizedString, CategoryEntry> _registeredCategories = [];
    private bool                                       _isDirty;

    public void Initialize()
    {
        _uiController             =  _uiManager.GetUIController<GameBarUIController>();
        _uiController.ItemPressed += OnItemPressed;

        _inputManager.OnKeyBindingAdded       += _ => QueueUpdate();
        _inputManager.OnKeyBindingRemoved     += _ => QueueUpdate();
        _inputManager.Contexts.ContextChanged += (_, _) => QueueUpdate();
    }

    public void FrameUpdate(FrameEventArgs args)
    {
        if (!_isDirty)
            return;

        if (!_client.RunLevel.IsInGameLike())
            return;

        _isDirty = false;
        Update();
    }

    private void OnItemPressed(LocalizedString category, LocalizedString item)
    {
        var categoryEntry = _registeredCategories[category];
        var itemDef       = categoryEntry.GetItems()[item];

        itemDef.Callback();
    }

    public CategoryEntry GetCategory(GameBarCategoryDef def)
    {
        if (_registeredCategories.TryGetValue(def.Name, out var category))
            return category;

        category = _registeredCategories[def.Name] = new(def, this);

        return category;
    }

    private void QueueUpdate() => _isDirty = true;

    private void Update()
    {
        var stateType = _stateManager.CurrentState.GetType();

        _registeredCategories = _registeredCategories
            .Where(c => c.Value.GetItems().Count > 0)
            .ToDictionary();

        UpdateBinds();

        var currentContext = _inputManager.Contexts.ActiveContext;

        var categories =
            _registeredCategories.AsEnumerable()
                .OrderByDescending(c => c.Value.Define.Priority)
                .ThenBy(c => c.Value.Define.Name.ToString())
                .Select(c =>
                {
                    var items = c.Value.GetItems()
                        .AsEnumerable()
                        .Where(i =>
                        {
                            if (i.Value.InGameState is { } gameState)
                                return gameState == stateType;

                            if (i.Value.Function is { } function)
                                return currentContext.FunctionExistsHierarchy(function);

                            return true;
                        })
                        .OrderByDescending(i => i.Value.Priority)
                        .ThenBy(i => i.Value.Name.ToString())
                        .Select(i =>
                        {
                            string? hotKey = null;

                            if (i.Value.Function is { } function && _inputManager.TryGetKeyBinding(function, out _))
                                hotKey = _inputManager.GetKeyFunctionButtonString(function);

                            return new GameBarPopup.Item(i.Key, hotKey);
                        })
                        .ToList();

                    return new UI.GameBar.Category(c.Key, items, c.Value.Define.IsIcon);
                })
                .Where(c => c.Items.Count > 0)
                .ToList();

        _uiController.Populate(categories);
    }

    private void UpdateBinds()
    {
        CommandBinds.Unregister<GameBarManager>();

        var builder = CommandBinds.Builder;

        foreach (var (_, category) in _registeredCategories)
        {
            var items = category.GetItems();

            foreach (var (_, item) in items)
            {
                if (item.Function is { } function)
                {
                    builder
                        .Bind(function, InputCmdHandler.FromDelegate(_ => item.Callback()));
                }
            }
        }

        builder.Register<GameBarManager>();
    }

    public sealed class CategoryEntry(GameBarCategoryDef def, GameBarManager parent)
    {
        public readonly GameBarCategoryDef Define = def;

        private readonly Dictionary<LocalizedString, GameBarItemDef> _items = [];

        public CategoryEntry RegisterItem(GameBarItemDef itemDef)
        {
            _items.TryAdd(itemDef.Name, itemDef);
            parent.QueueUpdate();

            return this;
        }

        public CategoryEntry RemoveItem(GameBarItemDef itemDef)
        {
            _items.Remove(itemDef.Name);
            parent.QueueUpdate();

            return this;
        }

        public CategoryEntry KeepItem(GameBarItemDef itemDef, bool when) =>
            when ? RegisterItem(itemDef) : RemoveItem(itemDef);

        public IReadOnlyDictionary<LocalizedString, GameBarItemDef> GetItems() => _items;
    }
}
