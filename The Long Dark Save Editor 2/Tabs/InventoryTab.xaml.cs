using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using The_Long_Dark_Save_Editor_2.Game_data;
using The_Long_Dark_Save_Editor_2.Helpers;
using The_Long_Dark_Save_Editor_2.Properties;

namespace The_Long_Dark_Save_Editor_2.Tabs
{
    /// <summary>
    /// Interaction logic for InventoryTab.xaml
    /// </summary>
    public partial class InventoryTab : UserControl
    {
        private MainWindow mainWindow;

        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register(nameof(ItemCount), typeof(int), typeof(InventoryTab), new PropertyMetadata(0));
        public int ItemCount { get => (int)GetValue(ItemCountProperty); set => SetValue(ItemCountProperty, value); }

        public static readonly DependencyProperty TotalWeightProperty =
            DependencyProperty.Register(nameof(TotalWeight), typeof(double), typeof(InventoryTab), new PropertyMetadata(0.0));
        public double TotalWeight { get => (double)GetValue(TotalWeightProperty); set => SetValue(TotalWeightProperty, value); }

        public InventoryTab()
        {
            InitializeComponent();
            mainWindow = MainWindow.Instance;
            UpdateAddItemList();
            Loaded += InventoryTab_Loaded;
        }

        private void InventoryTab_Loaded(object sender, RoutedEventArgs e)
        {
            if (mainWindow?.CurrentSave?.Global?.Inventory?.Items != null)
            {
                var items = mainWindow.CurrentSave.Global.Inventory.Items;
                items.CollectionChanged -= Items_CollectionChanged;
                items.CollectionChanged += Items_CollectionChanged;
                UpdateSummary();
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            var items = mainWindow?.CurrentSave?.Global?.Inventory?.Items;
            if (items == null)
            {
                ItemCount = 0;
                TotalWeight = 0;
                return;
            }
            ItemCount = items.Count;
            TotalWeight = items.Sum(i => i.Gear?.m_WeightKG ?? 0);
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

        private void cbItemCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAddItemList();
        }

        private void txtAddItemSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAddItemList();
        }

        private void UpdateAddItemList()
        {
            if (cbItemCategory == null)
                return;

            var category = cbItemCategory.SelectedValue as ItemCategory?;
            if (category == null)
                return;

            var filter = txtAddItemSearch?.Text?.Trim() ?? string.Empty;

            var items = new List<EnumerationMember>();
            foreach (var entry in ItemDictionary.itemInfo)
            {
                if (entry.Value.category == category && !entry.Value.hide)
                {
                    var description = Resources.ResourceManager.GetString(entry.Key) ?? entry.Key;
                    if (string.IsNullOrEmpty(filter) ||
                        description.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        entry.Key.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        items.Add(new EnumerationMember { Value = entry.Key, Description = description });
                    }
                }
            }

            items = items.OrderBy(item => item.Description).ToList();
            cbItem.ItemsSource = items;
            cbItem.SelectedIndex = items.Count > 0 ? 0 : -1;
        }

        private void PrintJsonClicked(object sender, RoutedEventArgs e)
        {
            // TODO!!
        }

<<<<<<< HEAD
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(ItemList.ItemsSource);
            if (view == null)
                return;

            var filter = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(filter))
            {
                view.Filter = null;
                return;
            }

            view.Filter = obj =>
            {
                var item = obj as InventoryItemSaveData;
                if (item == null)
                    return false;
                return (item.InGameName?.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    || (item.m_PrefabName?.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0);
            };
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

        private void cbSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(ItemList.ItemsSource);
            if (view == null || view.SortDescriptions == null)
                return;

            view.SortDescriptions.Clear();
            int idx = cbSortBy.SelectedIndex;
            if (idx == 0)
            {
                view.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("InGameName", ListSortDirection.Ascending));
            }
            else if (idx == 1)
            {
                view.SortDescriptions.Add(new SortDescription("InGameName", ListSortDirection.Ascending));
            }
            else if (idx == 2)
            {
                view.SortDescriptions.Add(new SortDescription("Gear.NormalizedCondition", ListSortDirection.Descending));
            }
        }
    }
}
