using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Managers;
using Content.Shared.Ghost;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.UIKit;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Utility;

namespace Content.Shared.UserInterface;

public sealed partial class ActivatableUISystem : EntitySystem
{
    [Dependency] private readonly ISharedAdminManager _adminManager = default!;
    [Dependency] private readonly ActionBlockerSystem _blockerSystem = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelistSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ActivatableUIComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<ActivatableUIComponent, UseInHandEvent>(OnUseInHand);
        SubscribeLocalEvent<ActivatableUIComponent, ActivateInWorldEvent>(OnActivate);
        SubscribeLocalEvent<ActivatableUIComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<ActivatableUIComponent, HandDeselectedEvent>(OnHandDeselected);
        SubscribeLocalEvent<ActivatableUIComponent, GotUnequippedHandEvent>(OnHandUnequipped);
        SubscribeLocalEvent<ActivatableUIComponent, BoundUIClosedEvent>(OnUIClose);
        SubscribeLocalEvent<ActivatableUIComponent, GetVerbsEvent<ActivationVerb>>(GetActivationVerb);
        SubscribeLocalEvent<ActivatableUIComponent, GetVerbsEvent<Verb>>(GetVerb);

        SubscribeLocalEvent<ActivatableUIComponent, GetVerbsEvent<AlternativeVerb>>(GetAltVerb); // Goobstation

        SubscribeLocalEvent<UserInterfaceComponent, OpenUiActionEvent>(OnActionPerform);

        InitializePower();
    }

    private void OnStartup(Entity<ActivatableUIComponent> ent, ref ComponentStartup args)
    {
        if (ent.Comp.Key == null)
        {
            Log.Error($"Missing UI Key for entity: {ToPrettyString(ent)}");
            return;
        }

        // TODO BUI
        // set interaction range to zero to avoid constant range checks.
        //
        // if (ent.Comp.InHandsOnly && _uiSystem.TryGetInterfaceData(ent.Owner, ent.Comp.Key, out var data))
        //     data.InteractionRange = 0;
    }

    private void OnActionPerform(EntityUid uid, UserInterfaceComponent component, OpenUiActionEvent args)
    {
        if (args.Handled || args.Key == null)
            return;

        args.Handled = _uiSystem.TryToggleUi(uid, args.Key, args.Performer);
    }


    private void GetActivationVerb(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<ActivationVerb> args)
    {
        if (component.VerbOnly || component.AltVerb || !ShouldAddVerb(uid, component, args)) // Goobstation
            return;

        args.Verbs.Add(new ActivationVerb
        {
            Act = () => InteractUI(args.User, uid, component),
            Text = Loc.GetString(component.VerbText),
            GlyphIcon = SymbolIcons.Preview,
        });
    }

    private void GetVerb(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<Verb> args)
    {
        if (!component.VerbOnly || component.AltVerb || !ShouldAddVerb(uid, component, args)) // Goobstation
            return;

        args.Verbs.Add(new Verb
        {
            Act = () => InteractUI(args.User, uid, component),
            Text = Loc.GetString(component.VerbText),
            GlyphIcon = SymbolIcons.Preview,
        });
    }

    private void GetAltVerb(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<AlternativeVerb> args) // Goobstation
    {
        if (!component.AltVerb || !ShouldAddVerb(uid, component, args))
            return;

        args.Verbs.Add(new AlternativeVerb
        {
            Act = () => InteractUI(args.User, uid, component),
            Text = Loc.GetString(component.VerbText),
            GlyphIcon = SymbolIcons.Preview,
        });
    }

    private bool ShouldAddVerb<T>(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<T> args) where T : Verb
    {
        if (!args.CanAccess)
            return false;

        if (_whitelistSystem.IsWhitelistFail(component.RequiredItems, args.Using ?? default)
            || _whitelistSystem.IsWhitelistFail(component.UserWhitelist, args.User))
            return false;

        if (component.RequiresComplex)
        {
            if (args.Hands == null)
                return false;

            if (component.InHandsOnly)
            {
                if (!_hands.IsHolding(args.User, uid, out var hand, args.Hands))
                    return false;

                if (component.RequireActiveHand && args.Hands.ActiveHand != hand)
                    return false;
            }
        }

        return args.CanInteract || HasComp<GhostComponent>(args.User) && !component.BlockSpectators;
    }

    private void OnUseInHand(EntityUid uid, ActivatableUIComponent component, UseInHandEvent args)
    {
        if (args.Handled)
            return;

        if (component.VerbOnly)
            return;

        if (component.RequiredItems != null)
            return;

        args.Handled = InteractUI(args.User, uid, component);
    }

    private void OnActivate(EntityUid uid, ActivatableUIComponent component, ActivateInWorldEvent args)
    {
        if (args.Handled || !args.Complex)
            return;

        if (component.VerbOnly)
            return;

        if (component.RequiredItems != null)
            return;

        args.Handled = InteractUI(args.User, uid, component);
    }

    private void OnInteractUsing(EntityUid uid, ActivatableUIComponent component, InteractUsingEvent args)
    {
        if (args.Handled)
            return;

        if (component.VerbOnly)
            return;

        if (_whitelistSystem.IsWhitelistFailOrNull(component.RequiredItems, args.Used) ||
            !_whitelistSystem.IsWhitelistFail(component.UserWhitelist, args.User))
            return;

        args.Handled = InteractUI(args.User, uid, component);
    }

    private void OnUIClose(EntityUid uid, ActivatableUIComponent component, BoundUIClosedEvent args)
    {
        var user = args.Actor;

        if (user != component.CurrentSingleUser)
            return;

        if (!Equals(args.UiKey, component.Key))
            return;

        SetCurrentSingleUser(uid, null, component);
    }

    private bool InteractUI(EntityUid user, EntityUid uiEntity, ActivatableUIComponent aui)
    {
        if (aui.Key == null || !_uiSystem.HasUi(uiEntity, aui.Key))
            return false;

        if (_uiSystem.IsUiOpen(uiEntity, aui.Key, user))
        {
            _uiSystem.CloseUi(uiEntity, aui.Key, user);
            return true;
        }

        if (!_blockerSystem.CanInteract(user, uiEntity) && (!HasComp<GhostComponent>(user) || aui.BlockSpectators))
            return false;

        if (aui.RequiresComplex)
        {
            if (!_blockerSystem.CanComplexInteract(user))
                return false;
        }

        if (aui.InHandsOnly)
        {
            if (!TryComp(user, out HandsComponent? hands))
                return false;

            if (!_hands.IsHolding(user, uiEntity, out var hand, hands))
                return false;

            if (aui.RequireActiveHand && hands.ActiveHand != hand)
                return false;
        }

        if (aui.AdminOnly && !_adminManager.IsAdmin(user))
            return false;

        if (aui.SingleUser && aui.CurrentSingleUser != null && user != aui.CurrentSingleUser)
        {
            var message = Loc.GetString("machine-already-in-use", ("machine", uiEntity));
            _popupSystem.PopupEntity(message, uiEntity, user);

            if (_uiSystem.IsUiOpen(uiEntity, aui.Key))
                return true;

            Log.Error($"Activatable UI has user without being opened? Entity: {ToPrettyString(uiEntity)}. User: {aui.CurrentSingleUser}, Key: {aui.Key}");
        }

        // If we've gotten this far, fire a cancellable event that indicates someone is about to activate this.
        // This is so that stuff can require further conditions (like power).
        var oae = new ActivatableUIOpenAttemptEvent(user);
        var uae = new UserOpenActivatableUIAttemptEvent(user, uiEntity);
        RaiseLocalEvent(user, uae);
        RaiseLocalEvent(uiEntity, oae);
        if (oae.Cancelled || uae.Cancelled)
            return false;

        // Give the UI an opportunity to prepare itself if it needs to do anything
        // before opening
        var bae = new BeforeActivatableUIOpenEvent(user);
        RaiseLocalEvent(uiEntity, bae);

        SetCurrentSingleUser(uiEntity, user, aui);
        _uiSystem.OpenUi(uiEntity, aui.Key, user);

        //Let the component know a user opened it so it can do whatever it needs to do
        var aae = new AfterActivatableUIOpenEvent(user, user);
        RaiseLocalEvent(uiEntity, aae);

        return true;
    }

    public void SetCurrentSingleUser(EntityUid uid, EntityUid? user, ActivatableUIComponent? aui = null)
    {
        if (!Resolve(uid, ref aui))
            return;

        if (!aui.SingleUser)
            return;

        aui.CurrentSingleUser = user;
        Dirty(uid, aui);

        RaiseLocalEvent(uid, new ActivatableUIPlayerChangedEvent());
    }

    public void CloseAll(EntityUid uid, ActivatableUIComponent? aui = null)
    {
        if (!Resolve(uid, ref aui, false))
            return;

        if (aui.Key == null)
        {
            Log.Error($"Encountered null key in activatable ui on entity {ToPrettyString(uid)}");
            return;
        }

        _uiSystem.CloseUi(uid, aui.Key);
    }

    private void OnHandDeselected(Entity<ActivatableUIComponent> ent, ref HandDeselectedEvent args)
    {
        if (ent.Comp.InHandsOnly && ent.Comp.RequireActiveHand)
            CloseAll(ent, ent);
    }

    private void OnHandUnequipped(Entity<ActivatableUIComponent> ent, ref GotUnequippedHandEvent args)
    {
        if (ent.Comp.InHandsOnly)
            CloseAll(ent, ent);
    }
}
