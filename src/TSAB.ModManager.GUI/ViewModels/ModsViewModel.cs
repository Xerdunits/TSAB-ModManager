using System.Collections.ObjectModel;
using System.Windows.Input;
using TSAB.ModManager.GUI.ViewModels.Helpers;

namespace TSAB.ModManager.GUI.ViewModels
{
    public class ModsViewModel : ViewModelBase
    {
        public ObservableCollection<ModItem> Mods { get; set; }
        public ModItem SelectedMod { get; set; }

        public ICommand ActivateModCommand { get; }
        public ICommand DeactivateModCommand { get; }

        public ModsViewModel()
        {
            // Ensure the list is initialized
            Mods = new ObservableCollection<ModItem>
            {
                new ModItem { Name = "Mod A", IsActive = false },
                new ModItem { Name = "Mod B", IsActive = true },
                new ModItem { Name = "Mod C", IsActive = false }
            };

            ActivateModCommand = new RelayCommand(_ => ActivateMod(), _ => SelectedMod != null && !SelectedMod.IsActive);
            DeactivateModCommand = new RelayCommand(_ => DeactivateMod(), _ => SelectedMod != null && SelectedMod.IsActive);
        }

        private void ActivateMod()
        {
            if (SelectedMod != null)
            {
                SelectedMod.IsActive = true;
                OnPropertyChanged(nameof(Mods)); // Ensure UI updates
            }
        }

        private void DeactivateMod()
        {
            if (SelectedMod != null)
            {
                SelectedMod.IsActive = false;
                OnPropertyChanged(nameof(Mods)); // Ensure UI updates
            }
        }
    }

    public class ModItem
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
