using Robust.Client.UserInterface.Controls;


namespace Content.Client.UIKit.Controls;


public sealed class UIKLabel : Label
{
    [Dependency] private readonly TypographyManager _typographyManager = null!;

    public FontType? FontType
    {
        get => _fontType;
        set
        {
            _fontType = value;
            UpdateAppearance();
        }
    }

    public TextStyle? TextStyle
    {
        get => _textStyle;
        set
        {
            _textStyle = value;
            UpdateAppearance();
        }
    }

    public FontWeight? FontWeight
    {
        get => _fontWeight;
        set
        {
            _fontWeight = value;
            UpdateAppearance();
        }
    }

    private FontType?   _fontType;
    private TextStyle?  _textStyle;
    private FontWeight? _fontWeight;

    public UIKLabel()
    {
        IoCManager.InjectDependencies(this);
    }

    private void UpdateAppearance()
    {
        var style = TextStyle ?? UIKit.TextStyle.Body;
        var fontWeight = FontWeight ?? style switch
        {
            UIKit.TextStyle.Headline => UIKit.FontWeight.Medium,
            UIKit.TextStyle.Title3 or UIKit.TextStyle.Title2 => UIKit
                .FontWeight.SemiBold,
            UIKit.TextStyle.Title1     => UIKit.FontWeight.Bold,
            UIKit.TextStyle.LargeTitle => UIKit.FontWeight.ExtraBold,
            _                                        => UIKit.FontWeight.Regular
        };

        FontOverride = _typographyManager.GetFont(
            type: FontType ?? UIKit.FontType.SansSerif,
            style: TextStyle ?? UIKit.TextStyle.Body,
            weight: FontWeight ?? fontWeight
        );

        if (style <= UIKit.TextStyle.Headline)
            FontColorOverride = Color.White;
    }
}
