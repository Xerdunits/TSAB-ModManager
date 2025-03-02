using System.Collections.ObjectModel;
using System.Windows.Input;
using TSAB.ModManager.Core.Helpers;
using System.Collections.Generic;
using TSAB.ModManager.GUI.ViewModels.Helpers;
using System.IO;

namespace TSAB.ModManager.GUI.ViewModels
{
    public class GameFilesViewModel : ViewModelBase
    {
        public ObservableCollection<GameFileItem> ModifiedFiles { get; set; }
        public GameFileItem SelectedFile { get; set; }

        public ICommand CheckFilesCommand { get; }
        public ICommand RestoreFileCommand { get; }
        public ICommand GenerateHashListCommand { get; }
        public ICommand RestoreAllCommand { get; }

        public GameFilesViewModel()
        {
            ModifiedFiles = new ObservableCollection<GameFileItem>();

            CheckFilesCommand = new RelayCommand(_ => CheckFiles());
            RestoreFileCommand = new RelayCommand(_ => RestoreFile(), _ => SelectedFile != null);
            GenerateHashListCommand = new RelayCommand(_ => GenerateHashList());
            RestoreAllCommand = new RelayCommand(_ => RestoreAllMissingFiles());

        }
        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        private bool _isCheckingFiles;
        public bool IsCheckingFiles
        {
            get => _isCheckingFiles;
            set
            {
                _isCheckingFiles = value;
                OnPropertyChanged(nameof(IsCheckingFiles));
            }
        }
        private async void CheckFiles()
        {
            IsCheckingFiles = true;
            ModifiedFiles.Clear();

            await Task.Run(() =>
            {
                List<string> modifiedFileList = VerificationManager.GetModifiedFiles();

                App.Current.Dispatcher.Invoke(() =>
                {
                    if (modifiedFileList.Count == 0)
                    {
                        ModifiedFiles.Add(new GameFileItem { FilePath = "All files are intact", Status = "OK" });
                    }
                    else
                    {
                        foreach (var file in modifiedFileList)
                        {
                            ModifiedFiles.Add(new GameFileItem { FilePath = file, Status = "Modified" });
                        }
                    }

                    IsCheckingFiles = false;
                    OnPropertyChanged(nameof(ModifiedFiles));
                });
            });
        }
        public void GenerateHashList()
        {
            string gamePath = GamePathFinder.DetectGamePath();
            if (string.IsNullOrEmpty(gamePath) || !Directory.Exists(gamePath))
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] Game path not found! Cannot generate hash list.");
                return;
            }

            string dataPath = Path.Combine(gamePath, "Data");
            if (!Directory.Exists(dataPath))
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Data folder not found: {dataPath}");
                return;
            }

            var hashDict = new Dictionary<string, string>();

            foreach (string file in Directory.GetFiles(dataPath, "*.*", SearchOption.AllDirectories))
            {
                hashDict[file] = VerificationManager.ComputeMD5(file);
            }

            ConfigManager.SaveHashList(hashDict);
            System.Diagnostics.Debug.WriteLine($"[INFO] Hash list generated with {hashDict.Count} entries.");
        }

        private async void RestoreFile()
        {
            if (SelectedFile != null)
            {
                bool restored = await Task.Run(() => BackupManager.RestoreFile(SelectedFile.FilePath));

                App.Current.Dispatcher.Invoke(() =>
                {
                    SelectedFile.Status = restored ? "Restored" : "Restore Failed";
                    System.Diagnostics.Debug.WriteLine(restored
                        ? $"[INFO] File restored: {SelectedFile.FilePath}"
                        : $"[ERROR] Could not restore file: {SelectedFile.FilePath}");

                    OnPropertyChanged(nameof(ModifiedFiles));
                });
            }
        }
        private void RestoreAllMissingFiles()
        {
            foreach (var file in ModifiedFiles)
            {
                if (file.Status == "Missing")
                {
                    bool restored = BackupManager.RestoreFile(file.FilePath);

                    if (restored)
                    {
                        file.Status = "Restored";
                        System.Diagnostics.Debug.WriteLine($"[INFO] Restored missing file: {file.FilePath}");
                    }
                    else
                    {
                        file.Status = "Restore Failed";
                        System.Diagnostics.Debug.WriteLine($"[ERROR] Could not restore missing file: {file.FilePath}");
                    }
                }
            }

            OnPropertyChanged(nameof(ModifiedFiles));
        }
    }

    public class GameFileItem
    {
        public string FilePath { get; set; }
        public string Status { get; set; }
    }
}
