using Content.Client.UIKit.Controls;
using Content.Client.UserInterface.Controls;
using Content.Shared.Chat;


namespace Content.Client.UserInterface.Systems.Chat.Controls;

public sealed class ChannelFilterCheckbox : UIKCheckBox
{
    public readonly ChatChannel Channel;

    public bool IsHidden => Parent == null;

    public ChannelFilterCheckbox(ChatChannel channel)
    {
        Channel = channel;
        Text = Loc.GetString($"hud-chatbox-channel-{Channel}");
    }

    private void UpdateText(int? unread)
    {
        var name = Loc.GetString($"hud-chatbox-channel-{Channel}");

        if (unread > 0)
            // todo: proper fluent stuff here.
            name += " (" + (unread > 9 ? "9+" : unread) + ")";

        Text = name;
    }

    public void UpdateUnreadCount(int? unread)
    {
        UpdateText(unread);
    }
}
