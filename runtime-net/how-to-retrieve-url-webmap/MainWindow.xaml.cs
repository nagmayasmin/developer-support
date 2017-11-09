using Esri.ArcGISRuntime.UI;
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
using System.Diagnostics;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System.Threading;

namespace RuntimeTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadWebMap();


        }

        private async Task LoadWebMap()

        {
            try
            {
                string webMapURL = "http://ess.maps.arcgis.com/home/item.html?id=4a537401d01f47519e448d8f39fb3a82";
                var webMap = await Map.LoadFromUriAsync(new Uri(webMapURL));
                MapViewControl.Map = webMap;

                Console.WriteLine(webMap.AllLayers[1].Name);

                var featLayer = webMap.AllLayers[1] as FeatureLayer;
                await featLayer.LoadAsync();

                ArcGISFeatureTable featTable = featLayer.FeatureTable as ArcGISFeatureTable;

                Console.WriteLine(featTable.LayerInfo.Source);

          

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Load Web Map Error:" + ex.Message);
            }
        }
        
       
    }

    
}
