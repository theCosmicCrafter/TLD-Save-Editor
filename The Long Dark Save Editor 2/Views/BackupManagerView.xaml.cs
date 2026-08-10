using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace The_Long_Dark_Save_Editor_2.Views
{
    public partial class BackupManagerView : UserControl
    {
        public class BackupEntry
        {
            public string FilePath { get; set; }
            public string DisplayName { get; set; }
            public DateTime LastWrite { get; set; }
        }

        public ObservableCollection<BackupEntry> Backups { get; set; }
        public string BackupDirectory { get; set; }

        public BackupManagerView(string backupDirectory)
        {
            BackupDirectory = backupDirectory;
            Backups = new ObservableCollection<BackupEntry>();
            LoadBackups();
            InitializeComponent();
            BackupList.ItemsSource = Backups;
        }

        private void LoadBackups()
        {
            Backups.Clear();
            if (!Directory.Exists(BackupDirectory))
                return;

            var files = new DirectoryInfo(BackupDirectory)
                .GetFiles("*.backup")
                .OrderByDescending(f => f.LastWriteTime);

            foreach (var file in files)
            {
                Backups.Add(new BackupEntry
                {
                    FilePath = file.FullName,
                    DisplayName = $"{file.LastWriteTime:yyyy-MM-dd HH:mm:ss}  ({file.Length / 1024} KB)",
                    LastWrite = file.LastWriteTime
                });
            }
        }

        private BackupEntry GetSelected()
        {
            return BackupList.SelectedItem as BackupEntry;
        }

        private void RestoreClicked(object sender, RoutedEventArgs e)
        {
            var entry = GetSelected();
            if (entry == null)
                return;

            var result = MessageBox.Show(
                "Restoring will replace the current save file with this backup.\n\nContinue?",
                "Restore Backup",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                var save = MainWindow.Instance?.CurrentSave;
                if (save == null)
                    return;

                File.Copy(entry.FilePath, save.path, true);
                MainWindow.Instance.RefreshClicked(this, null);
                MessageBox.Show("Backup restored successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to restore backup: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            var entry = GetSelected();
            if (entry == null)
                return;

            var result = MessageBox.Show(
                $"Delete backup:\n{Path.GetFileName(entry.FilePath)}?\n\nThis cannot be undone.",
                "Delete Backup",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                File.Delete(entry.FilePath);
                Backups.Remove(entry);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete backup: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenFolderClicked(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(BackupDirectory))
                Process.Start(BackupDirectory);
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
    }
}
