using System.Linq;
using Content.Client.UserInterface;
using Content.Client.UserInterface.Controls;
using Content.Shared.Ame.Components;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using UIKWindow = Content.Client.UIKit.Controls.UIKWindow;


namespace Content.Client.Ame.UI
{
    [GenerateTypedNameReferences]
    public sealed partial class AmeWindow : UIKWindow
    {
        public event Action<UiButton>? OnAmeButton;

        public AmeWindow()
        {
            RobustXamlLoader.Load(this);
            IoCManager.InjectDependencies(this);

            EjectButton.OnPressed += _ => OnAmeButton?.Invoke(UiButton.Eject);
            ToggleInjection.OnPressed += _ => OnAmeButton?.Invoke(UiButton.ToggleInjection);
            IncreaseFuelButton.OnPressed += _ => OnAmeButton?.Invoke(UiButton.IncreaseFuel);
            DecreaseFuelButton.OnPressed += _ => OnAmeButton?.Invoke(UiButton.DecreaseFuel);
        }

        /// <summary>
        /// Update the UI state when new state data is received from the server.
        /// </summary>
        /// <param name="state">State data sent by the server.</param>
        public void UpdateState(BoundUserInterfaceState state)
        {
            var castState = (AmeControllerBoundUserInterfaceState) state;

            // Disable all buttons if not powered
            if (ContentsContainer.Children.Any())
            {
                ButtonHelpers.SetButtonDisabledRecursive(ContentsContainer, !castState.HasPower);
                EjectButton.Disabled = false;
            }

            if (!castState.HasFuelJar)
            {
                EjectButton.Disabled = true;
                ToggleInjection.Disabled = true;
                FuelAmount.Text = Loc.GetString("ame-window-fuel-not-inserted-text");
            }
            else
            {
                EjectButton.Disabled = false;
                ToggleInjection.Disabled = false;
                FuelAmount.Text = $"{castState.FuelAmount}";
            }

            if (!castState.IsMaster)
            {
                ToggleInjection.Disabled = true;
            }

            if (!castState.Injecting)
            {
                InjectionStatus.Text = Loc.GetString("ame-window-engine-injection-status-not-injecting-label") + " ";
            }
            else
            {
                InjectionStatus.Text = Loc.GetString("ame-window-engine-injection-status-injecting-label") + " ";
            }

            CoreCount.Text = $"{castState.CoreCount}";
            InjectionAmount.Text = $"{castState.InjectionAmount}";
            // format power statistics to pretty numbers
            CurrentPowerSupply.Text = $"{castState.CurrentPowerSupply:N1}";
            TargetedPowerSupply.Text = $"{castState.TargetedPowerSupply:N1}";
        }
    }
}
