using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using The_Long_Dark_Save_Editor_2.Game_data;
using The_Long_Dark_Save_Editor_2.Helpers;
using Microsoft.Win32;

namespace The_Long_Dark_Save_Editor_2.Tabs
{
    /// <summary>
    /// Interaction logic for InventoryTab.xaml
    /// </summary>
    public partial class InventoryTab : UserControl
    {
        private MainWindow mainWindow;

        public InventoryTab()
        {
            InitializeComponent();
            mainWindow = MainWindow.Instance;

        }

        private void AddItemClicked(object sender, RoutedEventArgs e)
        {
            var prefabName = (string)cbItem.SelectedValue;

            if (prefabName == null)
                return;

            var itemInfo = ItemDictionary.itemInfo[prefabName];

            var item = new InventoryItemSaveData();
            var gear = GearItemSaveDataProxy.Create();
            JsonConvert.PopulateObject(itemInfo.defaultSerialized, gear);
            item.m_PrefabName = prefabName;
            item.Gear = gear;
            gear.m_HoursPlayed = mainWindow.CurrentSave.Global.TimeOfDay.m_HoursPlayedNotPausedProxy;
            mainWindow.CurrentSave.Global.Inventory.Items.Add(item);
            ItemList.SelectedItem = item;
        }

        private void DeleteItemClicked(object sender, RoutedEventArgs e)
        {
            var index = ItemList.SelectedIndex;
            mainWindow.CurrentSave.Global.Inventory.Items.Remove((InventoryItemSaveData)ItemList.SelectedValue);
            if (ItemList.Items.Count <= index)
                ItemList.SelectedIndex = index - 1;
            else
                ItemList.SelectedIndex = index;
        }

        private void RemoveAllClicked(object sender, RoutedEventArgs e)
        {
            mainWindow.CurrentSave.Global.Inventory.Items.Clear();
        }

        private void RepairAllClicked(object sender, RoutedEventArgs e)
        {
            foreach (var item in mainWindow.CurrentSave.Global.Inventory.Items)
            {
                var gear = item.Gear;
                gear.NormalizedCondition = 1;
                gear.m_WornOut = false;

                if (gear.FlareItem != null)
                    gear.FlareItem.m_StateProxy.SetValue(FlareState.Fresh);
                if (gear.TorchItem != null)
                    gear.TorchItem.m_StateProxy.SetValue(TorchState.Fresh);
            }
        }

        private void cbItem_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbItem.SelectedItem == null && cbItem.Items.Count > 0)
                cbItem.SelectedIndex = 0;
        }

        private void PrintJsonClicked(object sender, RoutedEventArgs e)
        {
            // TODO!!
        }

        private void ExportLoadoutClicked(object sender, RoutedEventArgs e)
        {
            if (mainWindow.CurrentSave == null)
                return;

            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json",
                FileName = "loadout.json"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(mainWindow.CurrentSave.Global.Inventory.Items, Formatting.Indented);
                    File.WriteAllText(dialog.FileName, json);
                }
                catch (Exception ex)
                {
                    ErrorDialog.Show("Failed to export loadout", ex.Message);
                }
            }
        }

        private void ImportLoadoutClicked(object sender, RoutedEventArgs e)
        {
            if (mainWindow.CurrentSave == null)
                return;

            var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = "json"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var json = File.ReadAllText(dialog.FileName);
                var imported = JsonConvert.DeserializeObject<List<InventoryItemSaveData>>(json);
                if (imported == null)
                    return;

                var currentItems = mainWindow.CurrentSave.Global.Inventory.Items;
                var existingIds = new HashSet<int>(currentItems.Select(i => i.Gear?.m_InstanceIDProxy ?? 0));

                foreach (var item in imported)
                {
                    if (item.Gear == null)
                        continue;

                    var r = new Random();
                    var id = r.Next();
                    while (existingIds.Contains(id))
                        id = r.Next();

                    existingIds.Add(id);
                    item.Gear.m_InstanceIDProxy = id;
                    item.Gear.m_HoursPlayed = mainWindow.CurrentSave.Global.TimeOfDay.m_HoursPlayedNotPausedProxy;
                    item.Gear.m_BeenInPlayerInventoryProxy = true;
                    item.Gear.m_NonInteractive = false;
                    currentItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show("Failed to import loadout", ex.Message);
            }
        }
    }
}
