using System.Numerics;
using Content.Client.UserInterface.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using static Robust.Client.UserInterface.Controls.BoxContainer;
using UIKWindow = Content.Client.UIKit.Controls.UIKWindow;


namespace Content.Client.Ghost.UI;

public sealed class ReturnToBodyMenu : UIKWindow
{
    public readonly Button DenyButton;
    public readonly Button AcceptButton;

    public ReturnToBodyMenu()
    {
        Title = Loc.GetString("ghost-return-to-body-title");

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
                            Text = Loc.GetString("ghost-return-to-body-text")
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

