using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   public class SettingsLibraryTypeVM : ObservableObject
   {

      public enLibraryType LibraryType { get; }
      public string Title { get; }
      public ObservableList<SettingsLibraryVM> Libraries { get; }

      public SettingsLibraryTypeVM(enLibraryType libraryType)
      {
         LibraryType = libraryType;
         Title = libraryType == enLibraryType.LocalDrive ? Resources.Translations.SETTINGS_LOCAL_DRIVE_TITLE : Resources.Translations.SETTINGS_ONE_DRIVE_TITLE;
         AddText = libraryType == enLibraryType.LocalDrive ? Resources.Translations.SETTINGS_LOCAL_DRIVE_ADD_COMMAND : Resources.Translations.SETTINGS_ONE_DRIVE_ADD_COMMAND;
         AddCommand = new Command(async () => await Add());
         Libraries = new ObservableList<SettingsLibraryVM>();
      }

      public string AddText { get; }
      public Command AddCommand { get; }
      Task Add() =>
         DependencyService.Get<IStoreService>()
            .AddLibraryAsync(LibraryType);

   }
}
