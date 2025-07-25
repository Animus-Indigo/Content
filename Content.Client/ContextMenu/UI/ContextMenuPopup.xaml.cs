using System.Numerics;
using Content.Client.UIKit.Controls;
using Content.Client.UserInterface.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Utility;

namespace Content.Client.ContextMenu.UI
{
    /// <summary>
    ///     The base context-menu pop-up window used for both the entity and verb menus.
    /// </summary>
    [GenerateTypedNameReferences]
    public sealed partial class ContextMenuPopup : UIKPopup
    {
        /// <summary>
        ///     How many items to list before limiting the size and adding a scroll bar.
        /// </summary>
        public const int MaxItemsBeforeScroll = 10;

        /// <summary>
        ///     If this pop-up is created by hovering over some element in another pop-up, this is that element.
        /// </summary>
        public ContextMenuElement? ParentElement;

        /// <summary>
        ///     This is the main body of the menu. The menu entries should be added to this object.
        /// </summary>
        public GridContainer MenuBody = new();

        private ContextMenuUIController _uiController;

        public ContextMenuPopup (ContextMenuUIController uiController, ContextMenuElement? parentElement) : base()
        {
            RobustXamlLoader.Load(this);

            _uiController = uiController;
            ParentElement = parentElement;

            // TODO xaml controls now have the access options -> re-xamlify all this.
            //XAML controls are private. So defining and adding MenuBody here instead.
            Scroll.AddChild(MenuBody);

            // Set Max Height based on MaxItemsBeforeScroll and the panel's style box
            MenuPanel.ForceRunStyleUpdate();
            MenuPanel.TryGetStyleProperty<StyleBox>(PanelContainer.StylePropertyPanel, out var box);
            var styleSize = (box?.MinimumSize ?? Vector2.Zero) / UIScale;
            MenuPanel.MaxHeight = MaxItemsBeforeScroll * (ContextMenuElement.ElementHeight) + styleSize.Y;

            UserInterfaceManager.ModalRoot.AddChild(this);
            MenuBody.OnChildRemoved += ctrl => _uiController.OnRemoveElement(this, ctrl);
            MenuBody.VSeparationOverride = 0;
            MenuBody.HSeparationOverride = 0;

            if (ParentElement != null)
            {
                DebugTools.Assert(ParentElement.SubMenu == null);
                ParentElement.SubMenu = this;
            }

            // ensure the menu-stack is properly updated when a pop-up looses focus or otherwise closes without going
            // through the menu presenter.
            OnPopupHide += () =>
            {
                if (ParentElement != null)
                    _uiController.CloseSubMenus(ParentElement.ParentMenu);
            };
        }

        protected override void Dispose(bool disposing)
        {
            MenuBody.OnChildRemoved -= ctrl => _uiController.OnRemoveElement(this, ctrl);
            ParentElement = null;
            base.Dispose(disposing);
        }
    }
}
