using System.Numerics;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using static Robust.Client.UserInterface.Controls.BoxContainer;
using UIKWindow = Content.Client.UIKit.Controls.UIKWindow;


namespace Content.Client.Psionics.UI
{
    public sealed class AcceptPsionicsWindow : UIKWindow
    {
        public readonly Button DenyButton;
        public readonly Button AcceptButton;

        public AcceptPsionicsWindow()
        {

            Title = Loc.GetString("accept-psionics-window-title");

            ContentsContainer.AddChild(new BoxContainer
            {
                Orientation = LayoutOrientation.Vertical,
                Children =
                {
                    new BoxContainer
                    {
                        Orientation = LayoutOrientation.Vertical,
                        Children =
                        {
                            (new Label()
                            {
                                Text = Loc.GetString("accept-psionics-window-prompt-text-part")
                            }),
                            new BoxContainer
                            {
                                Orientation = LayoutOrientation.Horizontal,
                                Align = AlignMode.Center,
                                Children =
                                {
                                    (AcceptButton = new Button
                                    {
                                        Text = Loc.GetString("accept-cloning-window-accept-button"),
                                    }),

                                    (new Control()
                                    {
                                        MinSize = new Vector2(20, 0)
                                    }),

                                    (DenyButton = new Button
                                    {
                                        Text = Loc.GetString("accept-cloning-window-deny-button"),
                                    })
                                }
                            },
                        }
                    },
                }
            });
        }
    }
}
