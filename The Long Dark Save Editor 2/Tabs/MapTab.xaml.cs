using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using The_Long_Dark_Save_Editor_2.Helpers;

namespace The_Long_Dark_Save_Editor_2.Tabs
{

    public partial class MapTab : UserControl
    {

        private MapInfo mapInfo;
        private bool mouseDown;
        private Point clickPosition;
        private Point lastMousePosition;

        private Point playerPosition;

        private string region;

        public MapTab()
        {
            InitializeComponent();

            MainWindow.Instance.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainWindow.Instance.CurrentSave))
                {
                    Debug.WriteLine("Currentsave changed");
                    if (MainWindow.Instance.CurrentSave == null)
                    {
                        region = null;
                        UpdateMap();
                        return;
                    }
                    region = MainWindow.Instance.CurrentSave.Boot.m_SceneName.Value;
                    playerPosition = new Point(MainWindow.Instance.CurrentSave.Global.PlayerManager.m_SaveGamePosition[0], MainWindow.Instance.CurrentSave.Global.PlayerManager.m_SaveGamePosition[2]);
                    if (!MapDictionary.MapExists(region))
                    {
                        region = MainWindow.Instance.CurrentSave.Global.GameManagerData.SceneTransition.m_LastOutdoorScene;
                        playerPosition = new Point(MainWindow.Instance.CurrentSave.Global.GameManagerData.SceneTransition.m_PosBeforeInteriorLoad[0], MainWindow.Instance.CurrentSave.Global.GameManagerData.SceneTransition.m_PosBeforeInteriorLoad[2]);
                    }
                    UpdateMap();
                    var saveGamePosition = MainWindow.Instance.CurrentSave.Global.PlayerManager.m_SaveGamePosition;
                    saveGamePosition.CollectionChanged += (sender2, e2) =>
                    {

                        if ((e2.NewStartingIndex == 0 && saveGamePosition[0] != (float)playerPosition.X) || (e2.NewStartingIndex == 2 && saveGamePosition[2] != (float)playerPosition.Y))
                        {
                            playerPosition.X = saveGamePosition[0];
                            playerPosition.Y = saveGamePosition[2];
                            UpdatePlayerPosition();
                        }
                    };
                    MainWindow.Instance.CurrentSave.Boot.m_SceneName.PropertyChanged += (sender2, e2) =>
                    {
                        if ((e2.PropertyName == "Value") && (region != MainWindow.Instance.CurrentSave.Boot.m_SceneName.Value) )
                        {
                            region = MainWindow.Instance.CurrentSave.Boot.m_SceneName.Value;
                            Debug.WriteLine("New region: " + region);
                            UpdateMap();
                        }
                    };
                }
            };

        }

        private void UpdateMap()
        {
            if (!IsLoaded)
                return;
            if (region == null)
            {
                mapImage.Source = null;
                mapInfo = null;
                player.Visibility = Visibility.Hidden;
                canvasLabel.Text = "";
                canvasLabel.Visibility = Visibility.Visible;
                return;
            }
            if (!MapDictionary.MapExists(region))
            {
                mapImage.Source = null;
                mapInfo = null;
                player.Visibility = Visibility.Hidden;
                canvasLabel.Text = "No map found for current region";
                canvasLabel.Visibility = Visibility.Visible;
                return;
            }
            player.Visibility = Visibility.Visible;
            canvasLabel.Visibility = Visibility.Hidden;

            mapInfo = MapDictionary.GetMapInfo(region);
            mapImage.Source = ((Image)Resources[region]).Source;
            mapImage.Width = mapInfo.width;
            mapImage.Height = mapInfo.height;

            double wScale = canvas.ActualWidth / mapInfo.width;
            double hScale = canvas.ActualHeight / mapInfo.height;
            scaleMap.ScaleX = Math.Max(Math.Min(wScale, hScale), 0.5);
            scaleMap.ScaleY = Math.Max(Math.Min(wScale, hScale), 0.5);

            scaleOfPlayerIcon.ScaleX = 1 / scaleMap.ScaleX;
            scaleOfPlayerIcon.ScaleY = 1 / scaleMap.ScaleY;
            UpdatePlayerPosition();
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mapInfo == null) return;

            mouseDown = true;
            clickPosition = e.GetPosition(canvas);
            lastMousePosition = clickPosition;
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mapInfo == null) return;

            mouseDown = false;
            canvas.ReleaseMouseCapture();
            if (e.GetPosition(canvas) == clickPosition)
            {
                playerPosition = mapInfo.ToRegion(e.GetPosition(mapImage));
                UpdatePlayerPosition();
                MainWindow.Instance.CurrentSave.Boot.m_SceneName.Value = region;
                MainWindow.Instance.CurrentSave.Global.PlayerManager.m_SaveGamePosition[0] = (float)playerPosition.X;
                MainWindow.Instance.CurrentSave.Global.PlayerManager.m_SaveGamePosition[2] = (float)playerPosition.Y;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (mapInfo == null) return;

            if (mouseDown)
            {
                canvas.CaptureMouse();
                var mousePos = e.GetPosition(canvas);

                translateMap.X += (mousePos.X - lastMousePosition.X);
                translateMap.Y += (mousePos.Y - lastMousePosition.Y);
                lastMousePosition = mousePos;
            }
        }

        private void canvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (mapInfo == null) return;

            double zoom = e.Delta > 0 ? .3 * scaleMap.ScaleX : -.3 * scaleMap.ScaleX;

            var x = e.GetPosition(mapLayer).X / mapLayer.ActualWidth;
            var y = e.GetPosition(mapLayer).Y / mapLayer.ActualHeight;
            x = Math.Max(Math.Min(x, 1), 0);
            y = Math.Max(Math.Min(y, 1), 0);
            var dX = (x - mapLayer.RenderTransformOrigin.X) * mapLayer.ActualWidth * (1 - scaleMap.ScaleX);
            var dY = (y - mapLayer.RenderTransformOrigin.Y) * mapLayer.ActualHeight * (1 - scaleMap.ScaleY);

            translateMap.X -= dX;
            translateMap.Y -= dY;
            mapLayer.RenderTransformOrigin = new Point(x, y);

            scaleMap.ScaleX += zoom;
            scaleMap.ScaleY += zoom;

            scaleOfPlayerIcon.ScaleX = 1 / scaleMap.ScaleX;
            scaleOfPlayerIcon.ScaleY = 1 / scaleMap.ScaleY;
        }

        private void UpdatePlayerPosition()
        {
            UpdatePlayerPosition(mapInfo.ToLayer(playerPosition));
        }

        private void UpdatePlayerPosition(Point layerPoint)
        {
            Canvas.SetLeft(player, layerPoint.X);
            Canvas.SetTop(player, layerPoint.Y);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMap();
        }

        private void ResetSceneClicked(object sender, RoutedEventArgs e)
        {
            var save = MainWindow.Instance?.CurrentSave;
            if (save?.Global == null)
                return;

            var sceneName = save.Boot.m_SceneName.Value;
            if (string.IsNullOrEmpty(sceneName))
                return;

            var result = MessageBox.Show(
                "This will delete the scene save file for the current region (" + sceneName + ").\n" +
                "All loot, containers, and objects in this region will reset to their original state.\n\n" +
                "The game will regenerate the scene data on next load.\n\nContinue?",
                "Reset Scene Data",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var saveDir = Path.GetDirectoryName(save.path);
            var sceneFile = Path.Combine(saveDir, sceneName + ".scene");

            try
            {
                if (File.Exists(sceneFile))
                    File.Delete(sceneFile);

                var st = save.Global.GameManagerData.SceneTransition;
                st.m_ForceNextSceneLoadTriggerScene = null;
                st.m_ForceSceneOnNextNavMapLoad = null;

                MessageBox.Show("Scene data reset. Save the game in the editor, then load in-game to regenerate the scene.",
                    "Scene Reset", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to reset scene data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
