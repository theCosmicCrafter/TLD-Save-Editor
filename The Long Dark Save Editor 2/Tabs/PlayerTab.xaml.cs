using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using The_Long_Dark_Save_Editor_2.Helpers;

namespace The_Long_Dark_Save_Editor_2.Tabs
{
    /// <summary>
    /// Interaction logic for PlayerTab.xaml
    /// </summary>
    public partial class PlayerTab : UserControl
    {
        public PlayerTab()
        {
            InitializeComponent();
            DependencyPropertyDescriptor
                .FromProperty(ComboBox.ItemsSourceProperty, typeof(RadioButton))
                .AddValueChanged(cbCurrentRegion, (s, e) =>
                {
                    foreach (var item in cbCurrentRegion.Items)
                    {
                        var em = item as EnumerationMember;
                        if ((string)em.Value == MainWindow.Instance.CurrentSave?.OriginalRegion)
                        {
                            cbCurrentRegion.SelectedItem = item;
                            break;
                        }
                    }
                });

            cbTeleport.Items.Clear();
            cbTeleport.Items.Add(new TeleportPreset { Name = "-- Select a location --" });
            foreach (var preset in TeleportPresets.Presets)
                cbTeleport.Items.Add(preset);
            cbTeleport.SelectedIndex = 0;
        }

        private void cbTeleport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTeleport.SelectedIndex <= 0)
                return;

            var preset = cbTeleport.SelectedItem as TeleportPreset;
            if (preset == null)
                return;

            var result = MessageBox.Show(
                "Teleport to " + preset.Name + "?\nThis will change your region and position.",
                "Teleport", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                cbTeleport.SelectedIndex = 0;
                return;
            }

            var save = MainWindow.Instance?.CurrentSave;
            if (save == null)
                return;

            save.Boot.m_SceneName.Value = preset.Region;

            var pos = save.Global.PlayerManager.m_SaveGamePosition;
            if (pos != null && pos.Length >= 3)
            {
                pos[0] = preset.X;
                pos[1] = preset.Z;
                pos[2] = preset.Y;
            }

            foreach (var item in cbCurrentRegion.Items)
            {
                var em = item as EnumerationMember;
                if ((string)em.Value == preset.Region)
                {
                    cbCurrentRegion.SelectedItem = item;
                    break;
                }
            }

            cbTeleport.SelectedIndex = 0;
        }
    }
}
