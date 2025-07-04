using Content.Client.Credits;
using Content.Client.UserInterface.Controls;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using UIKButton = Content.Client.UIKit.Controls.UIKButton;


namespace Content.Client.Info
{
    public sealed class DevInfoBanner : BoxContainer
    {
        public DevInfoBanner() {
            var buttons = new BoxContainer
            {
                Orientation = LayoutOrientation.Horizontal
            };
            AddChild(buttons);

            var uriOpener = IoCManager.Resolve<IUriOpener>();
            var cfg = IoCManager.Resolve<IConfigurationManager>();

            var bugReport = cfg.GetCVar(CCVars.InfoLinksBugReport);
            if (bugReport != "")
            {
                var reportButton = new UIKButton {Text = Loc.GetString("server-info-report-button")};
                reportButton.OnPressed += args => uriOpener.OpenUri(bugReport);
                buttons.AddChild(reportButton);
            }

            var creditsButton = new UIKButton {Text = Loc.GetString("server-info-credits-button")};
            creditsButton.OnPressed += args => new CreditsWindow().Open();
            buttons.AddChild(creditsButton);
        }
    }
}
