using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   public class SettingsLibraryTypeVM : ObservableObject
   {

      public enLibraryType LibraryType { get; }
      public string Title { get; }
      public ObservableList<SettingsLibraryVM> Libraries { get; }

      public SettingsLibraryTypeVM(enLibraryType libraryType, SettingsLibraryVM[] libraries)
      {
         LibraryType = libraryType;
         Libraries = new ObservableList<SettingsLibraryVM>(libraries);
         Title = libraryType == enLibraryType.LocalDrive ? Resources.Translations.SCREEN_SETTINGS_LOCAL_DRIVE_TITLE : Resources.Translations.SCREEN_SETTINGS_ONE_DRIVE_TITLE;
         AddText = libraryType == enLibraryType.LocalDrive ? Resources.Translations.SCREEN_SETTINGS_LOCAL_DRIVE_ADD_COMMAND : Resources.Translations.SCREEN_SETTINGS_ONE_DRIVE_ADD_COMMAND;
         AddCommand = new Command(async () => await Add());
         Helpers.Notify.LibraryAdd(this, library => AddLibrary(library));
         Helpers.Notify.LibraryRemove(this, library => RemoveLibrary(library));
      }

      public string AddText { get; }
      public Command AddCommand { get; }
      Task Add() =>
         DependencyService.Get<IStoreService>()
            .AddLibraryAsync(LibraryType);

      void AddLibrary(LibraryVM library)
      {
         if (library.Type != LibraryType) return;
         var libraryItem = Libraries.Where(x => x.Library.ID == library.ID).FirstOrDefault();
         if (libraryItem != null) return;
         Libraries.Add(new SettingsLibraryVM(library));
      }

      void RemoveLibrary(LibraryVM library)
      {
         if (library.Type != LibraryType) return;
         while (true)
         {
            var libraryItem = Libraries.Where(x => x.Library.ID == library.ID).FirstOrDefault();
            if (libraryItem == null) return;
            Libraries.Remove(libraryItem);
         }
      }

      public override void Dispose()
      {
         Helpers.Notify.LibraryAddUnsubscribe(this);
         Helpers.Notify.LibraryRemoveUnsubscribe(this);
         base.Dispose();
      }

   }
}
