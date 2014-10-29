using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace netfxver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();

            netFxVersionsList.ItemsSource = FrameworkVersion.GetInstalledFrameworkVersions();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DetailsCommand(FrameworkVersion version)
        {
            System.Diagnostics.Debugger.Break();
        }

        private void Details_OnClick(object sender, RoutedEventArgs e)
        {
            Button btnControl = sender as Button;
            if (btnControl != null && btnControl.DataContext is FrameworkVersion)
            {
                var ver = btnControl.DataContext as FrameworkVersion;
                VersionDetailsView.GetVersionDetailsView(ver).ShowDialog();
            }
        }
    }
}
