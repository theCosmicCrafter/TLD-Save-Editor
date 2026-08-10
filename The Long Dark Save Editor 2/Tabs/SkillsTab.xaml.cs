using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using The_Long_Dark_Save_Editor_2.Game_data;

namespace The_Long_Dark_Save_Editor_2.Tabs
{
    /// <summary>
    /// Interaction logic for SkillsTab.xaml
    /// </summary>
    public partial class SkillsTab : UserControl
    {
        public SkillsTab()
        {
            InitializeComponent();
        }

        private SkillsManagerSaveData SkillsManager
        {
            get { return MainWindow.Instance?.CurrentSave?.Global?.SkillsManager; }
        }

        private void MaxAllSkillsClicked(object sender, RoutedEventArgs e)
        {
            var sm = SkillsManager;
            if (sm == null)
                return;

            sm.Firestarting.m_Points = 100;
            sm.CarcassHarvesting.m_Points = 100;
            sm.Cooking.m_Points = 100;
            sm.IceFishing.m_Points = 100;
            sm.Rifle.m_Points = 100;
            sm.Archery.m_Points = 100;
            sm.ClothingRepair.m_Points = 100;
            sm.Revolver.m_Points = 100;
            sm.Gunsmith.m_Points = 100;

            RefreshSkills();
        }

        private void ResetAllSkillsClicked(object sender, RoutedEventArgs e)
        {
            var sm = SkillsManager;
            if (sm == null)
                return;

            sm.Firestarting.m_Points = 0;
            sm.CarcassHarvesting.m_Points = 0;
            sm.Cooking.m_Points = 0;
            sm.IceFishing.m_Points = 0;
            sm.Rifle.m_Points = 0;
            sm.Archery.m_Points = 0;
            sm.ClothingRepair.m_Points = 0;
            sm.Revolver.m_Points = 0;
            sm.Gunsmith.m_Points = 0;

            RefreshSkills();
        }

        private void RefreshSkills()
        {
            MainWindow.Instance?.OnPropertyChanged("CurrentSave");
        }
    }
}
