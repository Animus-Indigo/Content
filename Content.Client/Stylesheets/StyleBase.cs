using Content.Client.UIKit;
using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;


namespace Content.Client.Stylesheets;


public abstract class StyleBase
{
    public const string ClassHighDivider = "HighDivider";
    public const string ClassLowDivider = "LowDivider";
    public const string StyleClassLabelHeading = "LabelHeading";
    public const string StyleClassLabelSubText = "LabelSubText";
    public const string StyleClassItalic = "Italic";
    public const string StyleClassMonospace = "monospace";
    public const string StyleClassSerif = "serif";

    public const string ButtonOpenRight = "OpenRight";
    public const string ButtonOpenLeft = "OpenLeft";
    public const string ButtonOpenBoth = "OpenBoth";
    public const string ButtonSquare = "ButtonSquare";

    public const string ButtonCaution = "Caution";
    public const string ButtonDanger = "Danger";

    public const int DefaultGrabberSize = 10;

    public abstract Stylesheet Stylesheet { get; }

    protected StyleRule[] BaseRules { get; }

    protected StyleBoxTexture BaseButton { get; }
    protected StyleBoxTexture BaseButtonOpenRight { get; }
    protected StyleBoxTexture BaseButtonOpenLeft { get; }
    protected StyleBoxTexture BaseButtonOpenBoth { get; }
    protected StyleBoxTexture BaseButtonSquare { get; }

    protected StyleBoxTexture BaseAngleRect { get; }
    protected StyleBoxTexture AngleBorderRect { get; }

    protected StyleBase(IResourceCache resCache)
    {
        var typographyManager = IoCManager.Instance!.Resolve<TypographyManager>();

        var textureCloseButton = resCache.GetTexture("/Textures/Interface/Nano/cross.svg.png");

        // Button styles.
        var buttonTex = resCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
        BaseButton = new()
        {
            Texture = buttonTex
        };
        BaseButton.SetPatchMargin(StyleBox.Margin.All, 10);
        BaseButton.SetPadding(StyleBox.Margin.All, 1);
        BaseButton.SetContentMarginOverride(StyleBox.Margin.Vertical, 2);
        BaseButton.SetContentMarginOverride(StyleBox.Margin.Horizontal, 14);

        BaseButtonOpenRight = new(BaseButton)
        {
            Texture = new AtlasTexture(buttonTex, UIBox2.FromDimensions(new(0, 0), new(14, 24)))
        };
        BaseButtonOpenRight.SetPatchMargin(StyleBox.Margin.Right, 0);
        BaseButtonOpenRight.SetContentMarginOverride(StyleBox.Margin.Right, 8);
        BaseButtonOpenRight.SetPadding(StyleBox.Margin.Right, 2);

        BaseButtonOpenLeft = new(BaseButton)
        {
            Texture = new AtlasTexture(buttonTex, UIBox2.FromDimensions(new(10, 0), new(14, 24)))
        };
        BaseButtonOpenLeft.SetPatchMargin(StyleBox.Margin.Left, 0);
        BaseButtonOpenLeft.SetContentMarginOverride(StyleBox.Margin.Left, 8);
        BaseButtonOpenLeft.SetPadding(StyleBox.Margin.Left, 1);

        BaseButtonOpenBoth = new(BaseButton)
        {
            Texture = new AtlasTexture(buttonTex, UIBox2.FromDimensions(new(10, 0), new(3, 24)))
        };
        BaseButtonOpenBoth.SetPatchMargin(StyleBox.Margin.Horizontal, 0);
        BaseButtonOpenBoth.SetContentMarginOverride(StyleBox.Margin.Horizontal, 8);
        BaseButtonOpenBoth.SetPadding(StyleBox.Margin.Right, 2);
        BaseButtonOpenBoth.SetPadding(StyleBox.Margin.Left, 1);

        BaseButtonSquare = new(BaseButton)
        {
            Texture = new AtlasTexture(buttonTex, UIBox2.FromDimensions(new(10, 0), new(3, 24)))
        };
        BaseButtonSquare.SetPatchMargin(StyleBox.Margin.Horizontal, 0);
        BaseButtonSquare.SetContentMarginOverride(StyleBox.Margin.Horizontal, 8);
        BaseButtonSquare.SetPadding(StyleBox.Margin.Right, 2);
        BaseButtonSquare.SetPadding(StyleBox.Margin.Left, 1);

        BaseAngleRect = new()
        {
            Texture = buttonTex
        };
        BaseAngleRect.SetPatchMargin(StyleBox.Margin.All, 10);

        AngleBorderRect = new()
        {
            Texture = resCache.GetTexture("/Textures/Interface/Nano/geometric_panel_border.svg.96dpi.png")
        };
        AngleBorderRect.SetPatchMargin(StyleBox.Margin.All, 10);

        var vScrollBarGrabberNormal = new StyleBoxFlat
        {
            BackgroundColor = Color.Gray.WithAlpha(0.35f), ContentMarginLeftOverride = DefaultGrabberSize,
            ContentMarginTopOverride = DefaultGrabberSize
        };
        var vScrollBarGrabberHover = new StyleBoxFlat
        {
            BackgroundColor = new Color(140, 140, 140).WithAlpha(0.35f),
            ContentMarginLeftOverride = DefaultGrabberSize,
            ContentMarginTopOverride = DefaultGrabberSize
        };
        var vScrollBarGrabberGrabbed = new StyleBoxFlat
        {
            BackgroundColor = new Color(160, 160, 160).WithAlpha(0.35f),
            ContentMarginLeftOverride = DefaultGrabberSize,
            ContentMarginTopOverride = DefaultGrabberSize
        };

        var hScrollBarGrabberNormal = new StyleBoxFlat
        {
            BackgroundColor = Color.Gray.WithAlpha(0.35f), ContentMarginTopOverride = DefaultGrabberSize
        };
        var hScrollBarGrabberHover = new StyleBoxFlat
        {
            BackgroundColor = new Color(140, 140, 140).WithAlpha(0.35f),
            ContentMarginTopOverride = DefaultGrabberSize
        };
        var hScrollBarGrabberGrabbed = new StyleBoxFlat
        {
            BackgroundColor = new Color(160, 160, 160).WithAlpha(0.35f),
            ContentMarginTopOverride = DefaultGrabberSize
        };


        BaseRules =
        [
            // Default font.
            new(
                new SelectorElement(null, null, null, null),
                [
                    new("font", typographyManager.GetFont(FontType.SansSerif))
                ]),

            // Default italic font.
            new(
                new SelectorElement(null, [StyleClassItalic,], null, null),
                [
                    new(
                        "font",
                        typographyManager.GetFont(FontType.SansSerif, modifier: FontModifier.Italic))
                ]),
            // Default serif font.
            new(
                new SelectorElement(null, [StyleClassSerif,], null, null),
                [
                    new("font", typographyManager.GetFont(FontType.Serif))
                ]),
            // Default mono font
            new(
                new SelectorElement(null, [StyleClassMonospace,], null, null),
                [
                    new("font", typographyManager.GetFont(FontType.Mono))
                ]),

            // Window close button base texture.
            new(
                new SelectorElement(
                    typeof(TextureButton),
                    [DefaultWindow.StyleClassWindowCloseButton,],
                    null,
                    null),
                [
                    new(TextureButton.StylePropertyTexture, textureCloseButton)
                ]),
            // Window close button hover.
            new(
                new SelectorElement(
                    typeof(TextureButton),
                    [DefaultWindow.StyleClassWindowCloseButton,],
                    null,
                    [TextureButton.StylePseudoClassHover,]),
                [
                    new(Control.StylePropertyModulateSelf, Color.FromHex("#7F3636"))
                ]),
            // Window close button pressed.
            new(
                new SelectorElement(
                    typeof(TextureButton),
                    [DefaultWindow.StyleClassWindowCloseButton,],
                    null,
                    [TextureButton.StylePseudoClassPressed,]),
                [
                    new(Control.StylePropertyModulateSelf, Color.FromHex("#753131"))
                ]),

            // Scroll bars
            new(
                new SelectorElement(typeof(VScrollBar), null, null, null),
                [
                    new(
                        ScrollBar.StylePropertyGrabber,
                        vScrollBarGrabberNormal)
                ]),

            new(
                new SelectorElement(typeof(VScrollBar), null, null, [ScrollBar.StylePseudoClassHover,]),
                [
                    new(
                        ScrollBar.StylePropertyGrabber,
                        vScrollBarGrabberHover)
                ]),

            new(
                new SelectorElement(typeof(VScrollBar), null, null, [ScrollBar.StylePseudoClassGrabbed,]),
                [
                    new(
                        ScrollBar.StylePropertyGrabber,
                        vScrollBarGrabberGrabbed)
                ]),

            new(
                new SelectorElement(typeof(HScrollBar), null, null, null),
                [
                    new(
                        ScrollBar.StylePropertyGrabber,
                        hScrollBarGrabberNormal)
                ]),

            new(
                new SelectorElement(typeof(HScrollBar), null, null, [ScrollBar.StylePseudoClassHover,]),
                [
                    new(
                        ScrollBar.StylePropertyGrabber,
                        hScrollBarGrabberHover)
                ]),

            new(
                new SelectorElement(typeof(HScrollBar), null, null, [ScrollBar.StylePseudoClassGrabbed,]),
                [
                    new(
                        ScrollBar.StylePropertyGrabber,
                        hScrollBarGrabberGrabbed)
                ])
        ];
    }
}
