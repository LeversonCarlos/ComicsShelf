using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Splash
{
   public class SplashVM : BaseVM
   {
      public IStoreService Store { get => DependencyService.Get<IStoreService>(); }

      public string LibraryID { get; set; }
      public SplashVM SetLibraryID(string libraryID)
      {
         LibraryID = libraryID;
         return this;
      }

      public string FileID { get; set; }
      public SplashVM SetFileID(string fileID)
      {
         FileID = fileID;
         return this;
      }

      LibraryVM Library { get; set; }
      public SplashVM LoadLibrary()
      {
         if (string.IsNullOrEmpty(LibraryID)) { return null; }
         Library = Store.GetLibrary(LibraryID);
         if (Library == null) { return null; }
         return this;
      }

      public ItemVM[] ItemsList { get; set; }
      public void LoadItems()
      {
         if (string.IsNullOrEmpty(FileID)) { return; }

         var libraryItems = Store.GetLibraryItems(Library);
         if (libraryItems?.Length == 0) { return; }

         var libraryItem = libraryItems.Where(x => x.EscapedID == FileID).FirstOrDefault();
         if (libraryItem == null) { return; }

         ItemsList = libraryItems
            .Where(item => item.FolderPath == libraryItem.FolderPath)
            .OrderBy(item => item.FullText)
            .ToArray();
      }

      public override Task OnAppearing()
      {
         return base.OnAppearing();
      }

      public override Task OnDisappearing()
      {
         return base.OnDisappearing();
      }

   }
}
