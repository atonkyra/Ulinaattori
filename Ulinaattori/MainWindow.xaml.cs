using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ulinaattori.Listener;
using Ulinaattori.Plugins;

namespace Ulinaattori {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private CheckBox corsairAPIEnabled = new CheckBox();
        private CheckBox useNotifications = new CheckBox();

        private MessageListener zl = null;
        private CUEPlugin cp = null;

        private TaskbarIcon tbi = null;

        public MainWindow() {
            InitializeComponent();
            this.tbi = new TaskbarIcon();
            tbi.TrayLeftMouseDown += Tbi_TrayLeftMouseDown;
            tbi.TrayBalloonTipClicked += Tbi_TrayBalloonTipClicked;
            tbi.TrayBalloonTipClosed += Tbi_TrayBalloonTipClosed;
            tbi.TrayMouseDoubleClick += Tbi_TrayMouseDoubleClick;
            {
                var bnd = new Binding();
                bnd.Source = Properties.Settings.Default;
                bnd.Path = new PropertyPath("zmqURL");
                bnd.Mode = BindingMode.TwoWay;
                bnd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                this.zmqURL.SetBinding(TextBox.TextProperty, bnd);
                this.zmqURL.Text = Properties.Settings.Default.zmqURL;
            }
            {
                var bnd = new Binding();
                bnd.Source = Properties.Settings.Default;
                bnd.Path = new PropertyPath("useNotifications");
                bnd.Mode = BindingMode.TwoWay;
                bnd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                this.useNotifications.Content = "Use notifications";
                this.useNotifications.SetBinding(CheckBox.IsCheckedProperty, bnd);
                this.optionStackPanel.Children.Add(this.useNotifications);
            }
            {
                var bnd = new Binding();
                bnd.Source = Properties.Settings.Default;
                bnd.Path = new PropertyPath("corsairAPIEnabled");
                bnd.Mode = BindingMode.TwoWay;
                bnd.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                this.corsairAPIEnabled.Content = "Corsair LED API";
                this.corsairAPIEnabled.SetBinding(CheckBox.IsCheckedProperty, bnd);
                this.optionStackPanel.Children.Add(this.corsairAPIEnabled);
            }
        }

        private void Tbi_TrayMouseDoubleClick(object sender, RoutedEventArgs e) {
            if(this.zl != null)
                this.zl.shutdown();
            System.Windows.Application.Current.Shutdown();
        }

        private void Tbi_TrayBalloonTipClosed(object sender, RoutedEventArgs e) {
        }

        private void Tbi_TrayBalloonTipClicked(object sender, RoutedEventArgs e) {
            if(this.cp != null)
                this.cp.raiseClearFlag();
        }

        private void Tbi_TrayLeftMouseDown(object sender, RoutedEventArgs e) {
            if(this.cp != null)
                this.cp.raiseClearFlag();
        }

        private void onStartListening(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.Save();
            try {
                zl = new MessageListener(zmqURL.Text);
                zl.onPlainMessage += msgListener_onPlainMessage;
                this.Hide();
            } catch(Exception exc) {
                this.tbi.ShowBalloonTip("ZMQ Initialization Failure", exc.Message, BalloonIcon.Error);
            }
            if(Properties.Settings.Default.corsairAPIEnabled) {
                try {
                    this.cp = new CUEPlugin();
                    zl.onPlainMessage += this.cp.onMessage;
                    zl.onJSONMessage += this.cp.onMessage;
                } catch(Exception exc) {
                    this.tbi.ShowBalloonTip("CUE Initialization Failure", exc.Message, BalloonIcon.Error);
                }
            }
        }

        private void msgListener_onPlainMessage(string topic, string message) {
            if(Properties.Settings.Default.useNotifications)
                tbi.ShowBalloonTip(topic, message, BalloonIcon.Info);
        }
    }
}
