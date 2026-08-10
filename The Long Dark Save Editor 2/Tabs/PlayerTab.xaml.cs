using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using The_Long_Dark_Save_Editor_2.Game_data;
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

        private void FeatCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null)
                return;

            var save = MainWindow.Instance?.CurrentSave;
            if (save == null)
                return;

            if (save.Global.FeatsEnabled == null)
                save.Global.FeatsEnabled = new FeatEnabledTrackerSaveData { m_FeatsEnabledThisSandbox = new System.Collections.Generic.List<EnumWrapper<FeatType>>() };

            if (save.Global.FeatsEnabled.m_FeatsEnabledThisSandbox == null)
                save.Global.FeatsEnabled.m_FeatsEnabledThisSandbox = new System.Collections.Generic.List<EnumWrapper<FeatType>>();

            FeatType feat;
            if (!Enum.TryParse(checkBox.Tag?.ToString(), out feat))
                return;

            var list = save.Global.FeatsEnabled.m_FeatsEnabledThisSandbox;
            var existing = list.FirstOrDefault(x => x.Value == feat);

            if (checkBox.IsChecked == true)
            {
                if (existing == null)
                    list.Add(new EnumWrapper<FeatType>(feat));
            }
            else
            {
                if (existing != null)
                    list.Remove(existing);
            }
        }

        private void GodModeClicked(object sender, RoutedEventArgs e)
        {
            var save = MainWindow.Instance?.CurrentSave;
            if (save?.Global == null)
                return;

            save.Global.Condition.m_NeverDieProxy = true;
            save.Global.Condition.m_Invulnerable = true;
            save.Global.Condition.m_CurrentHPProxy = 100f;
            save.Global.Thirst.m_CurrentThirstProxy = 100f;
            save.Global.Fatigue.m_CurrentFatigueProxy = 100f;
            save.Global.Freezing.m_CurrentFreezingProxy = 100f;
            save.Global.Hunger.m_CurrentReserveCaloriesProxy = 2500f;

            MainWindow.Instance.PropertyChanged?.Invoke(MainWindow.Instance, new PropertyChangedEventArgs("CurrentSave"));
        }

        private void CopyPositionClicked(object sender, RoutedEventArgs e)
        {
            var save = MainWindow.Instance?.CurrentSave;
            if (save?.Global?.PlayerManager?.m_SaveGamePosition == null)
                return;

            var pos = save.Global.PlayerManager.m_SaveGamePosition;
            var text = $"X: {pos[0]}, Y: {pos[2]}, Z: {pos[1]}";
            try
            {
                Clipboard.SetText(text);
            }
            catch { }
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
