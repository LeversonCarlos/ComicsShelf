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
      public ObservableList<LibrarySetting> Libraries { get; }

      public LibrariesSetting(enLibraryType libraryType)
      {
         LibraryType = libraryType;
         Title = libraryType == enLibraryType.LocalDrive ? R.Settings.LOCAL_DRIVE_LIBRARIES_TITLE : "ONE_DRIVE_LIBRARIES_TITLE";
         AddText = libraryType == enLibraryType.LocalDrive ? R.Settings.ADD_LOCAL_DRIVE_LIBRARY_COMMAND : "R.Settings.ADD_ONE_DRIVE_LIBRARY_COMMAND";
         AddCommand = new Command(async () => await Add());
         Libraries = new ObservableList<LibrarySetting>(GetLibraryList());
         Notifyers.Notify.LibraryAdd(library => AddLibrary(library));
         Notifyers.Notify.LibraryRemove(library => RemoveLibrary(library));
      }

      LibrarySetting[] GetLibraryList() => DependencyService
         .Get<IStoreService>()
         .GetLibraries()
         .Where(x => x.Type == LibraryType)
         .Select(x => new LibrarySetting(x))
         .ToArray();

      public string AddText { get; }
      public Command AddCommand { get; set; }
      Task Add() =>
         DependencyService.Get<IStoreService>()
            .AddLibraryAsync(LibraryType);

      void AddLibrary(LibraryVM library) =>
         Libraries.Add(new LibrarySetting(library));

      void RemoveLibrary(LibraryVM library) =>
         Libraries.Remove(Libraries.Where(x => x.Library.ID == library.ID).FirstOrDefault());

   }
}
