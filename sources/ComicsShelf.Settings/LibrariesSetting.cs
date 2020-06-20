using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Settings
{
   public class LibrariesSetting : ObservableObject
   {

      public enLibraryType LibraryType { get; }
      public string Title { get; }

      public LibrariesSetting(enLibraryType libraryType)
      {
         LibraryType = libraryType;
         Title = libraryType == enLibraryType.LocalDrive ? Strings.LOCAL_DRIVE_LIBRARIES_TITLE : Strings.ONE_DRIVE_LIBRARIES_TITLE;
         AddText = libraryType == enLibraryType.LocalDrive ? Strings.ADD_LOCAL_DRIVE_LIBRARY_COMMAND : Strings.ADD_ONE_DRIVE_LIBRARY_COMMAND;
         AddCommand = new Command(async () => await Add());
         Libraries = new ObservableList<LibrarySetting>(GetLibraryList());
         Notifyers.Notify.LibraryAdd(library => AddLibrary(library));
         Notifyers.Notify.LibraryRemove(library => RemoveLibrary(library));
      }

      public ObservableList<LibrarySetting> Libraries { get; }
      LibrarySetting[] GetLibraryList() => DependencyService
         .Get<IStoreService>()
         .GetLibraries()
         .Where(x => x.Type == LibraryType)
         .Select(x => new LibrarySetting(x))
         .ToArray();

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

   }
}
