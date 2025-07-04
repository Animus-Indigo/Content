using Content.Client.UserInterface.Controls;
using UIKWindow = Content.Client.UIKit.Controls.UIKWindow;


namespace Content.Client.Administration.UI.CustomControls
{
    public sealed class UICommandButton : CommandButton
    {
        public  Type?        WindowType { get; set; }
        private UIKWindow? _window;

        protected override void Execute(ButtonEventArgs obj)
        {
            if (WindowType == null)
                return;
            _window = (UIKWindow) IoCManager.Resolve<IDynamicTypeFactory>().CreateInstance(WindowType);
            _window?.OpenCentered();
        }
    }
}
