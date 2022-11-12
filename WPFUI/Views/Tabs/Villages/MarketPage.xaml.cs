using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.ViewModels.Tabs.Villages;

using System.Diagnostics;

namespace WPFUI.Views.Tabs.Villages
{
    /// <summary>
    /// Interaction logic for MarketPage.xaml
    /// </summary>
    public partial class MarketPage : ReactivePage<MarketViewModel>
    {
        public MarketPage()
        {
            ViewModel = new();
            InitializeComponent();

            SendOutLimit.ViewModel = new("SendOutLimit");
            SendTo.ViewModel = new("SendCoordinates");
            // Bote
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.IsSendExcessResources, v => v.IsSendExcessResources.IsChecked).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessWood, v => v.SendOutLimit.ViewModel.Wood).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessClay, v => v.SendOutLimit.ViewModel.Clay).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessIron, v => v.SendOutLimit.ViewModel.Iron).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessCrop, v => v.SendOutLimit.ViewModel.Crop).DisposeWith(d);

                this.Bind(ViewModel, vm => vm.Settings.SendExcessToX, v => v.SendTo.ViewModel.XCoordinate).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Settings.SendExcessToY, v => v.SendTo.ViewModel.YCoordinate).DisposeWith(d);

                Disposable.Create(() => ViewModel.OnDeactived()).DisposeWith(d);

                ViewModel.OnActived();
            });
        }
    }
}