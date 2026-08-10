using System;
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
    }
}
