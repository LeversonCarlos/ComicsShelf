using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   public class LibrariesSetting : ObservableObject
   {

      public enLibraryType LibraryType { get; }
      public string Title { get; }

      public LibrariesSetting(enLibraryType libraryType)
      {
         LibraryType = libraryType;
         Title = libraryType == enLibraryType.LocalDrive ? Resources.Translations.SETTINGS_LOCAL_DRIVE_TITLE : Resources.Translations.SETTINGS_ONE_DRIVE_TITLE;
         AddText = libraryType == enLibraryType.LocalDrive ? Resources.Translations.SETTINGS_LOCAL_DRIVE_ADD_COMMAND : Resources.Translations.SETTINGS_ONE_DRIVE_ADD_COMMAND;
         AddCommand = new Command(async () => await Add());
         Libraries = new ObservableList<LibrarySetting>();
         LoadLibraryList();
         Helpers.Notify.LibraryAdd(library => AddLibrary(library));
         Helpers.Notify.LibraryRemove(library => RemoveLibrary(library));
      }

      public ObservableList<LibrarySetting> Libraries { get; }
      void LoadLibraryList()
      {
         var libraryList = DependencyService
            .Get<IStoreService>()
            ?.GetLibraries()
            .Where(x => x.Type == LibraryType)
            .Select(x => new LibrarySetting(x))
            .ToArray();
         if (libraryList != null) Libraries.AddRange(libraryList);
      }

      public string AddText { get; }
      public Command AddCommand { get; }
      Task Add() =>
         DependencyService.Get<IStoreService>()
            .AddLibraryAsync(LibraryType);

      void AddLibrary(LibraryVM library)
      {
         if (library.Type != LibraryType) { return; }
         Libraries.Add(new LibrarySetting(library));
      }

      void RemoveLibrary(LibraryVM library)
      {
         if (library.Type != LibraryType) { return; }
         var libraryItem = Libraries.Where(x => x.Library.ID == library.ID).FirstOrDefault();
         if (libraryItem == null) { return; }
         Libraries.Remove(libraryItem);
      }

      public override void Dispose()
      {
         Helpers.Notify.LibraryAddUnsubscribe();
         Helpers.Notify.LibraryRemoveUnsubscribe();
         base.Dispose();
      }

   }
}
