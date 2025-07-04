using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Client.Administration.UI;
using Content.Client.Administration.UI.Tabs.ObjectsTab;
using Content.Client.Administration.UI.Tabs.PanicBunkerTab;
using Content.Client.Administration.UI.Tabs.BabyJailTab;
using Content.Client.Administration.UI.Tabs.PlayerTab;
using Content.Client.GameBar;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.Mapping;
using Content.Client.UserInterface.Controls;
using Content.Client.Verbs.UI;
using Content.Shared.Administration.Events;
using Content.Shared.Input;
using JetBrains.Annotations;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;

namespace Content.Client.UserInterface.Systems.Admin;

[UsedImplicitly]
public sealed class AdminUIController : UIController,
    IOnStateEntered<GameplayState>,
    IOnStateEntered<LobbyState>,
    IOnStateEntered<MappingState>,
    IOnSystemChanged<AdminSystem>
{
    [Dependency] private readonly IClientAdminManager       _adminManager      = null!;
    [Dependency] private readonly IClientConGroupController _conGroups         = default!;
    [Dependency] private readonly IClientConsoleHost        _conHost           = default!;
    [Dependency] private readonly VerbMenuUIController      _verb              = default!;
    [Dependency] private readonly GameBarManager         _gameBarManager = null!;

    private AdminMenuWindow?   _window;
    private PanicBunkerStatus? _panicBunker;
    private BabyJailStatus?    _babyJail;

    private GameBarItemDef _windowItem;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<PanicBunkerChangedEvent>(OnPanicBunkerUpdated);
        SubscribeNetworkEvent<BabyJailChangedEvent>(OnBabyJailUpdated);

        _windowItem = new(
            new("global-menu-admin-window-item"),
            Callback: Toggle,
            Function: ContentKeyFunctions.OpenAdminMenu
        );

        _adminManager.AdminStatusUpdated += OnAdminStatusUpdated;
    }

    private void OnPanicBunkerUpdated(PanicBunkerChangedEvent msg, EntitySessionEventArgs args)
    {
        var showDialog = _panicBunker == null && msg.Status.Enabled;
        _panicBunker = msg.Status;
        _window?.PanicBunkerControl.UpdateStatus(msg.Status);

        if (showDialog)
        {
            UIManager.CreateWindow<PanicBunkerStatusWindow>().OpenCentered();
        }
    }

    private void OnBabyJailUpdated(BabyJailChangedEvent msg, EntitySessionEventArgs args)
    {
        var showDialog = _babyJail == null && msg.Status.Enabled;
        _babyJail = msg.Status;
        _window?.BabyJailControl.UpdateStatus(msg.Status);

        if (showDialog)
        {
            UIManager.CreateWindow<BabyJailStatusWindow>().OpenCentered();
        }
    }

    public void OnStateEntered(GameplayState state)
    {
        EnsureWindow();
        OnAdminStatusUpdated();
    }

    public void OnStateEntered(LobbyState state)
    {
        EnsureWindow();
        OnAdminStatusUpdated();
    }

    public void OnStateEntered(MappingState state)
    {
        EnsureWindow();
        OnAdminStatusUpdated();
    }

    public void OnSystemLoaded(AdminSystem system)
    {
        EnsureWindow();
    }

    private void OnAdminStatusUpdated()
    {
        var isAdmin = _conGroups.CanAdminMenu();

        _gameBarManager
            .GetCategory(GameBarCategory.Admin)
            .KeepItem(_windowItem, when: isAdmin);
    }

    public void OnSystemUnloaded(AdminSystem system)
    {
        if (_window != null)
            _window.Dispose();
    }

    private void EnsureWindow()
    {
        if (_window is { Disposed: false })
            return;

        if (_window?.Disposed ?? false)
            OnWindowDisposed();

        _window = UIManager.CreateWindow<AdminMenuWindow>();
        LayoutContainer.SetAnchorPreset(_window, LayoutContainer.LayoutPreset.Center);

        if (_panicBunker != null)
            _window.PanicBunkerControl.UpdateStatus(_panicBunker);

        /*
         * TODO: Remove baby jail code once a more mature gateway process is established. This code is only being issued as a stopgap to help with potential tiding in the immediate future.
         */

        if (_babyJail != null)
            _window.BabyJailControl.UpdateStatus(_babyJail);

        _window.PlayerTabControl.OnEntryKeyBindDown += PlayerTabEntryKeyBindDown;
        _window.ObjectsTabControl.OnEntryKeyBindDown += ObjectsTabEntryKeyBindDown;
        _window.OnDisposed += OnWindowDisposed;
    }

    private void OnWindowDisposed()
    {
        if (_window == null)
            return;

        _window.PlayerTabControl.OnEntryKeyBindDown -= PlayerTabEntryKeyBindDown;
        _window.ObjectsTabControl.OnEntryKeyBindDown -= ObjectsTabEntryKeyBindDown;
        _window.OnDisposed -= OnWindowDisposed;
        _window = null;
    }

    private void Toggle()
    {
        if (_window is {IsOpen: true})
        {
            _window.Close();
        }
        else if (_conGroups.CanAdminMenu())
        {
            _window?.Open();
        }
    }

    private void PlayerTabEntryKeyBindDown(GUIBoundKeyEventArgs args, ListData? data)
    {
        if (data is not PlayerListData {Info: var info})
            return;

        if (info.NetEntity == null)
            return;

        var entity = info.NetEntity.Value;
        var function = args.Function;

        if (function == EngineKeyFunctions.UIClick)
            _conHost.ExecuteCommand($"vv {entity}");
        else if (function == EngineKeyFunctions.UIRightClick)
            _verb.OpenVerbMenu(entity, true);
        else
            return;

        args.Handle();
    }

    private void ObjectsTabEntryKeyBindDown(GUIBoundKeyEventArgs args, ListData? data)
    {
        if (data is not ObjectsListData { Info: var info })
            return;

        var uid = info.Entity;
        var function = args.Function;

        if (function == EngineKeyFunctions.UIClick)
            _conHost.ExecuteCommand($"vv {uid}");
        else if (function == EngineKeyFunctions.UIRightClick)
            _verb.OpenVerbMenu(uid, true);
        else
            return;

        args.Handle();
    }
}
