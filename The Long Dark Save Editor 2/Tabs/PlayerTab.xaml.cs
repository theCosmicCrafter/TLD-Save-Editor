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
    }
}
