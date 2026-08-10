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

        private void ClearCheatsFlagClicked(object sender, RoutedEventArgs e)
        {
            var save = MainWindow.Instance?.CurrentSave;
            if (save?.Global?.PlayerManager == null)
                return;

            var result = MessageBox.Show(
                "Clear the 'Cheats Used' flag?\nThis may restore achievements/feats eligibility.",
                "Clear Cheats Flag", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                save.Global.PlayerManager.m_CheatsUsed = false;
        }
    }
}
