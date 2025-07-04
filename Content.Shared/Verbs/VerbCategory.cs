using Content.Shared.UIKit;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Verbs
{
    /// <summary>
    ///     Contains combined name and icon information for a verb category.
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class VerbCategory
    {
        public readonly string Text;

        public readonly SpriteSpecifier? SpriteIconIcon;

        public readonly string? GlypIcon;

        /// <summary>
        ///     Columns for the grid layout that shows the verbs in this category. If <see cref="IconsOnly"/> is false,
        ///     this should very likely be set to 1.
        /// </summary>
        public int Columns = 1;

        /// <summary>
        ///     If true, the members of this verb category will be shown in the context menu as a row of icons without
        ///     any text.
        /// </summary>
        /// <remarks>
        ///     For example, the 'Rotate' category simply shows two icons for rotating left and right.
        /// </remarks>
        public readonly bool IconsOnly;

        public VerbCategory(string text, bool iconsOnly = false)
        {
            Text = Loc.GetString(text);
            IconsOnly = iconsOnly;
        }

        public VerbCategory(string text, string glyphIcon, bool iconsOnly = false)
        {
            Text      = Loc.GetString(text);
            GlypIcon  = glyphIcon;
            IconsOnly = iconsOnly;
        }

        public VerbCategory(string text, SpriteSpecifier? spriteIcon, bool iconsOnly = false)
        {
            Text = Loc.GetString(text);
            SpriteIconIcon = spriteIcon;
            IconsOnly = iconsOnly;
        }

        public static readonly VerbCategory Admin =
            new("verb-categories-admin", SymbolIcons.Person);

        public static readonly VerbCategory Antag =
            new("verb-categories-antag", new SpriteSpecifier.Rsi(new("/Textures/Objects/Weapons/Melee/e_sword.rsi"), "icon"), iconsOnly: true)
                { Columns = 5 };

        public static readonly VerbCategory Examine =
            new("verb-categories-examine", SymbolIcons.Search);

        public static readonly VerbCategory Debug =
            new("verb-categories-debug", SymbolIcons.BugReport);

        public static readonly VerbCategory Eject =
            new("verb-categories-eject", SymbolIcons.LeftPanelOpen);

        public static readonly VerbCategory Insert =
            new("verb-categories-insert", SymbolIcons.LeftPanelClose);

        public static readonly VerbCategory Buckle =
            new("verb-categories-buckle", SymbolIcons.AirlineSeatReclineNormal);

        public static readonly VerbCategory Unbuckle =
            new("verb-categories-unbuckle", SymbolIcons.AirlineSeatReclineNormal);

        public static readonly VerbCategory Rotate =
            new("verb-categories-rotate", SymbolIcons.Sync, iconsOnly: true)
                { Columns = 5 };

        public static readonly VerbCategory Smite =
            new("verb-categories-smite", SymbolIcons.Bolt, iconsOnly: true)
                { Columns = 6 };

        public static readonly VerbCategory Tricks =
            new("verb-categories-tricks", SymbolIcons.WandStars, iconsOnly: true)
                { Columns = 5 };

        public static readonly VerbCategory SetTransferAmount =
            new("verb-categories-transfer", SymbolIcons.Colors);

        public static readonly VerbCategory Split = new("verb-categories-split");

        public static readonly VerbCategory InstrumentStyle = new("verb-categories-instrument-style");

        public static readonly VerbCategory ChannelSelect = new("verb-categories-channel-select");

        public static readonly VerbCategory SetSensor = new("verb-categories-set-sensor");

        public static readonly VerbCategory Lever = new("verb-categories-lever");

        public static readonly VerbCategory SelectType = new("verb-categories-select-type");

        public static readonly VerbCategory SelectFaction = new("verb-categories-select-faction");

        public static readonly VerbCategory Rename = new("verb-categories-rename");

        public static readonly VerbCategory Interaction = new("verb-categories-interaction");

        public static readonly VerbCategory BloodSpells = new("verb-categories-blood-cult",
            new SpriteSpecifier.Rsi(new ResPath("/Textures/WhiteDream/BloodCult/actions.rsi"), "blood_spells"));
        public static readonly VerbCategory PowerLevel = new("verb-categories-power-level");

        // Shitmed - Starlight Abductors
        public static readonly VerbCategory Switch = new("verb-categories-switch", SymbolIcons.ChevronRight);

        public static readonly VerbCategory GenderChange = new("verb-categories-gender-change");
        public static readonly VerbCategory SexChange = new("verb-categories-sex-change");
    }
}
