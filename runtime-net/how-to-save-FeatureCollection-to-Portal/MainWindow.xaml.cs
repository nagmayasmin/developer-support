using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace FeatureCollectionSaveToPortal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string FeatureLayerUrl = "http://sampleserver6.arcgisonline.com/arcgis/rest/services/Wildfire/FeatureServer/0";

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                // Create a new map with the oceans basemap and add it to the map view
                Map myMap = new Map(Basemap.CreateOceans());
                MyMapView.Map = myMap;

                // Call a function that will create a new feature collection layer from a service query
               SaveFeatureCollectionFromQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create feature collection layer: " + ex.Message, "Error");
            }
        }

        private async void SaveFeatureCollectionFromQuery()
        {
            Debug.WriteLine("Creating feature collection from the service...");
            
            // Create a service feature table to get features from
            ServiceFeatureTable featTable = new ServiceFeatureTable(new Uri(FeatureLayerUrl));

            // Create a query to get all features in the table
            QueryParameters queryParams = new QueryParameters();
            queryParams.WhereClause = "1=1";

            // Query the table to get all features
            FeatureQueryResult featureResult = await featTable.QueryFeaturesAsync(queryParams);

            // Create a new feature collection table from the result features
            FeatureCollectionTable collectTable = new FeatureCollectionTable(featureResult);

            // Create a feature collection and add the table
            FeatureCollection featCollection = new FeatureCollection();
            featCollection.Tables.Add(collectTable);


            var tokenServiceUri = new Uri("https://www.arcgis.com/sharing/rest");  // url for generating token

            AuthenticationManager authManager = AuthenticationManager.Current;
            var cred = await authManager.GenerateCredentialAsync(tokenServiceUri, "*******", "******");

            // Open a portal item containing a feature collection
            ArcGISPortal portal = await ArcGISPortal.CreateAsync(tokenServiceUri, cred);

            List<string> str = new List<string>();
            str.Add("Save Feature collection");

           
            FeatureCollectionLayer featCollectionTable = new FeatureCollectionLayer(featCollection);
            await featCollectionTable.LoadAsync();
            MyMapView.Map.OperationalLayers.Add(featCollectionTable);

            Debug.WriteLine("Create a feature layer named, Feature Collection to Portal, and saving to portal...");

            try
            {
                await featCollection.SaveAsAsync(portal, null, "Feature Collection to Portal", "FeatureCollection", str);
                MessageBox.Show("Feature Collection saved to portal");
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}
