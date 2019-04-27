using ComicsShelf.Helpers;
using System;
using Xamarin.Forms;

namespace ComicsShelf
{
   public class ShellVM : BaseVM
   {

      public ShellVM()
      {
         this.NewLibraryCommand = new Command((libraryType) => this.NewLibrary((Libraries.LibraryType)libraryType));
         this.DeleteLibraryCommand = new Command((shellItem) => this.DeleteLibrary((ShellItem)shellItem));
      }

      public Command NewLibraryCommand { get; set; }
      private void NewLibrary(Libraries.LibraryType libraryType)
      {
         this.IsBusy = true;
         try
         {
            var libraryStore = DependencyService.Get<Libraries.LibraryStore>();
            var library = new Libraries.LibraryModel
            {
               Key = DateTime.Now.Ticks.ToString(),
               Type = libraryType, 
               Description = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };
            libraryStore.AddLibrary(library);
         }
         catch (Exception) { throw; }
         this.IsBusy = false;
      }

      public Command DeleteLibraryCommand { get; set; }
      private void DeleteLibrary(ShellItem shellItem)
      {
         this.IsBusy = true;
         try
         {
            var libraryStore = DependencyService.Get<Libraries.LibraryStore>();
            libraryStore.DeleteLibrary(shellItem);
         }
         catch (Exception) { throw; }
         this.IsBusy = false;
      }


   }
}
