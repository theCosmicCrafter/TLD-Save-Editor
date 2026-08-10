using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using The_Long_Dark_Save_Editor_2.Game_data;

namespace The_Long_Dark_Save_Editor_2.Tabs
{
    public partial class RawJsonTab : UserControl
    {
        public RawJsonTab()
        {
            InitializeComponent();
            tgEnableEdit.Checked += (s, e) => { txtJson.IsReadOnly = false; btnApply.IsEnabled = true; };
            tgEnableEdit.Unchecked += (s, e) => { txtJson.IsReadOnly = true; btnApply.IsEnabled = false; };
            Loaded += RawJsonTab_Loaded;
        }

        private void RawJsonTab_Loaded(object sender, RoutedEventArgs e)
        {
            LoadJson();
        }

        private void LoadJson()
        {
            var save = MainWindow.Instance?.CurrentSave;
            if (save?.Global == null)
            {
                txtJson.Text = "No save loaded.";
                return;
            }

            var json = JsonConvert.SerializeObject(save.Global, Formatting.Indented);
            txtJson.Text = json;
        }

        private void ReloadClicked(object sender, RoutedEventArgs e)
        {
            LoadJson();
        }

        private void ApplyClicked(object sender, RoutedEventArgs e)
        {
            var save = MainWindow.Instance?.CurrentSave;
            if (save?.Global == null)
                return;

            var result = MessageBox.Show(
                "Apply raw JSON changes to the Global save data?\n" +
                "WARNING: Invalid JSON or incorrect field types may corrupt your save.\n" +
                "A backup will be created on save.",
                "Apply Raw JSON", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var obj = JsonConvert.DeserializeObject<GlobalSaveGameFormat>(txtJson.Text);
                if (obj == null)
                {
                    MessageBox.Show("Failed to parse JSON: result was null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var saveObj = save.Global;
                foreach (var prop in typeof(GlobalSaveGameFormat).GetProperties())
                {
                    if (prop.CanWrite && prop.CanRead)
                    {
                        var val = prop.GetValue(obj);
                        prop.SetValue(saveObj, val);
                    }
                }

                MessageBox.Show("Raw JSON applied. Save the game to persist changes.", "Applied",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Failed to parse JSON: " + ex.Message, "JSON Parse Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
