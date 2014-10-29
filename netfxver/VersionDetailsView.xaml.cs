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
using System.Windows.Shapes;

namespace netfxver
{
    /// <summary>
    /// Interaction logic for VersionDetailsView.xaml
    /// </summary>
    public partial class VersionDetailsView : Window
    {
        public VersionDetailsView()
        {
            InitializeComponent();
        }

        public static VersionDetailsView GetVersionDetailsView(FrameworkVersion netfxVersion)
        {
            var result = new VersionDetailsView();
            result.DataContext = netfxVersion;
            result.PropertiesListView.ItemsSource = new List<Tuple<String, String>>
            {
                new Tuple<String, String>("Friendly Name", netfxVersion.FriendlyName),
                new Tuple<String, String>("Registry Key", netfxVersion.KeyName),
                new Tuple<String, String>("Version", netfxVersion.Version),
                new Tuple<String, String>("Service Pack Version", netfxVersion.ServicePack.ToString())
            };
            return result;
        }
    }
}
