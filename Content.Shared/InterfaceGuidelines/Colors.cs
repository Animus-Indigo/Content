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

namespace Content.Shared.InterfaceGuidelines;


public static class Colors
{
    public static readonly Color BlackLight = Color.FromHex("#1E1C20");
    public static readonly Color Black      = Color.FromHex("#19171b");
    public static readonly Color BlackDark  = Color.FromHex("#0D0C0E");

    public static readonly Color BlueLight = Color.FromHex("#3F5FFD");
    public static readonly Color Blue      = Color.FromHex("#3351E3");
    public static readonly Color BlueDark  = Color.FromHex("#2A3255");

    public static readonly Color GrayLight = Color.FromHex("#544F5C");
    public static readonly Color Gray      = Color.FromHex("#36333B");
    public static readonly Color GrayDark  = Color.FromHex("#262329");

    public static readonly Color IndigoLight = Color.FromHex("#B23EFF");
    public static readonly Color Indigo      = Color.FromHex("#8113C9");
    public static readonly Color IndigoDark  = Color.FromHex("#3A2747");

    public static readonly Color RedLight = Color.FromHex("#FF3333");
    public static readonly Color Red      = Color.FromHex("#DE1313");
    public static readonly Color RedDark  = Color.FromHex("#572727");

    public static readonly Color Yellow = Color.FromHex("#E8BD13");

    public static readonly Color Green  = Color.FromHex("#24AF08");

    public static Color AccentLight => IndigoLight;
    public static Color Accent      => Indigo;
    public static Color AccentDark  => IndigoDark;

    #region Window

    public static          Color WindowBackground => BlackLight;
    public static readonly Color WindowBorder      = Color.Black;
    public static readonly Color WindowInsetBorder = Color.FromHex("#363438");

    public static readonly Color WindowTitle = Color.FromHex("#AAAAAA");
    public static          Color WindowTitlebarBackground => Gray;
    public static          Color WindowTitlebarBorder     => WindowBorder;
    public static readonly Color WindowTitlebarInsetBorder = Color.FromHex("#4b4951");

    public static readonly Color WindowTitlebarCloseButton = Color.FromHex("#ff6565");
    public static          Color WindowTitlebarHelpButton => Yellow;

    #endregion

    #region Button

    public static          Color ButtonLabel => Color.White;
    public static readonly Color ButtonDisabledLabel = Color.White.WithAlpha(0.15f);

    #region Default

    public static readonly Color ButtonBackground         = Color.FromHex("#5d5666");
    public static readonly Color ButtonPressedBackground  = Color.FromHex("#887F96");
    public static readonly Color ButtonDisabledBackground = Color.FromHex("#2C2931");

    #endregion

    #region Accent

    public static Color ButtonAccentBackground         => Accent;
    public static Color ButtonAccentPressedBackground  => AccentLight;
    public static Color ButtonAccentDisabledBackground => AccentDark;

    #endregion

    #region Danger

    public static Color ButtonDangerBackground         => Red;
    public static Color ButtonDangerPressedBackground  => RedLight;
    public static Color ButtonDangerDisabledBackground => RedDark;

    #endregion

    #endregion

    #region Button Option

    public static Color OptionButtonSelectedItemLabel         => Color.White;
    public static Color OptionButtonSelectedItemLabelDisabled => ButtonDisabledLabel;

    #endregion

    #region Slider

    public static readonly Color SliderBackground = Color.FromHex("#2C2930");
    public static          Color SliderGrabberBackground         => ButtonBackground;
    public static          Color SliderGrabberPressedBackground  => ButtonPressedBackground;
    public static          Color SliderGrabberDisabledBackground => ButtonDisabledBackground;

    #endregion

    #region Label

    public static readonly Color Label         = Color.FromHex("#E1E1E1");
    public static readonly Color LabelTitle1   = Color.White;
    public static readonly Color LabelTitle2   = Color.White;
    public static readonly Color LabelTitle3   = Color.White;
    public static readonly Color LabelHeadline = Color.White;

    #endregion

    #region Line Edit

    public static          Color LineEditBackground => GrayDark;
    public static readonly Color LineEditBorder      = Color.FromHex("#3b3940");
    public static readonly Color LineEditInsetBorder = Color.FromHex("#5c5a5f");
    public static          Color LineEditPlaceholder => ButtonDisabledLabel;
    public static          Color LineEditCursor      => Accent;
    public static          Color LineEditSelection   => Accent.WithAlpha(0.25f);

    #endregion

    #region Check Box

    public static Color CheckBoxBackground        => ButtonBackground;
    public static Color CheckBoxCheckedBackground => Accent;

    #endregion

    #region Popup

    public static Color PopupBackground  => WindowBackground;
    public static Color PopupBorder      => WindowBorder;
    public static Color PopupInsetBorder => WindowInsetBorder;

    #endregion

    #region Tab Container

    public static          Color TabContainerBackground => Black;
    public static readonly Color TabContainerBorder = Color.FromHex("#302f31");

    #endregion

    #region Chat

    public static Color ChatBorder      => WindowBorder;
    public static Color ChatInsetBorder => WindowInsetBorder;
    public static Color ChatBackground  => BlackLight;

    #endregion
}
