using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
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

namespace RouteCreation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        

        GraphicsOverlay _routeGraphicsOverlay;

        // Secured route service
        private const string ROUTE_SOURCE_URI = "https://route.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World";

        // Non-secured route service
        //  private const string ROUTE_SOURCE_URI = "http://sampleserver6.arcgisonline.com/arcgis/rest/services/NetworkAnalysis/SanDiego/NAServer/Route";

        private const string PORTAL_URL = "https://www.arcgis.com/sharing/rest/generateToken";

        public MainWindow()
        {
            InitializeComponent();
            // Set up the AuthenticationManager to use OAuth for secure ArcGIS Online requests
            UpdateAuthenticationManager();

            Initialize();
        }

        private void UpdateAuthenticationManager()
        {
            // Register the server information with the AuthenticationManager
            ServerInfo serverInfo = new ServerInfo
            {
                ServerUri = new Uri(PORTAL_URL),
                OAuthClientInfo = new OAuthClientInfo
                {
                    ClientId = "VT1YyOgV0pn7teuF", // set your own Client_ID
                    RedirectUri = new Uri("urn:ietf:wg:oauth:2.0:oob")
                }
            };

            // If a client secret has been configured, set the authentication type to OAuthAuthorizationCode
            // Otherwise, use OAuthImplicit

            serverInfo.TokenAuthenticationType = TokenAuthenticationType.OAuthImplicit;

            // Register this server with AuthenticationManager
            AuthenticationManager.Current.RegisterServer(serverInfo);

            // Use the OAuthAuthorize class in this project to handle OAuth communication
            AuthenticationManager.Current.OAuthAuthorizeHandler = new OAuthAuthorizeHandler();
            AuthenticationManager.Current.ChallengeHandler = new ChallengeHandler(PortalSecurity.Challenge);


        }

        private async void Initialize()
        {
            try
            {
                _routeGraphicsOverlay = new GraphicsOverlay();
                SimpleLineSymbol routeSymbol = new SimpleLineSymbol();
                routeSymbol.Color = Color.FromRgb(255, 0, 0);
                routeSymbol.Width = 3;

                SimpleRenderer routeLayerRenderer = new SimpleRenderer(routeSymbol);
                _routeGraphicsOverlay.Renderer = routeLayerRenderer;



                var routeSourceUri = new Uri(ROUTE_SOURCE_URI);
                var _routeTask = await RouteTask.CreateAsync(routeSourceUri);

                // Create new Map with basemap
                Map myMap = new Map(Basemap.CreateStreets());

                // Create and set initial map area
                Envelope initialLocation = new Envelope(
                    -12211308.778729, 4645116.003309, -12208257.879667, 4650542.535773,
                    SpatialReferences.WebMercator);
                myMap.InitialViewpoint = new Viewpoint(initialLocation);

                // Assign the map to the MapView
                MyMapView.Map = myMap;

                RouteParameters routeParams = await _routeTask.CreateDefaultParametersAsync();
                routeParams.OutputSpatialReference = SpatialReferences.WebMercator;
                routeParams.ReturnDirections = true;
                routeParams.ReturnRoutes = true;


                // create a Stop for my location
                var myLocation = new MapPoint(-13034379.7137452, 3858390.14190331, SpatialReferences.WebMercator);
                var stop1 = new Stop(myLocation);


                // create a Stop for your location
                var yourLocation = new MapPoint(-13032763.2362459, 3860880.60006008, SpatialReferences.WebMercator);
                var stop2 = new Stop(yourLocation);


                // assign the stops to the route parameters
                var stopPoints = new List<Stop> { stop1, stop2 };
                routeParams.SetStops(stopPoints);

                RouteResult routeResult = await _routeTask.SolveRouteAsync(routeParams);

                if (routeResult.Routes.Count > 0)
                {

                    var route = routeResult.Routes.First().RouteGeometry;
                    _routeGraphicsOverlay.Graphics.Add(new Graphic(route));
                    MyMapView.GraphicsOverlays.Add(_routeGraphicsOverlay);

                    await MyMapView.SetViewpointAsync(new Viewpoint(route.Extent));

                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        // Map initialization logic is contained in MapViewModel.cs
    }
}
