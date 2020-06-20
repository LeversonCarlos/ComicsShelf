using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Settings
{
   public class LibrariesSetting : ObservableObject
   {

      public enLibraryType LibraryType { get; }

      public LibrariesSetting(enLibraryType libraryType)
      {
         LibraryType = libraryType;

         AddCommand = new Command(async () => await Add());
      }

      public Command AddCommand { get; set; }
      Task Add() =>
         DependencyService.Get<IStoreService>()
            .AddLibraryAsync(LibraryType);


   }
}
