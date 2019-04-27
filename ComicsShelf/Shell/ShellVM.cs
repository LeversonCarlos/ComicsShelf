using ComicsShelf.Helpers;
using System;
using Xamarin.Forms;

namespace ComicsShelf
{
   public class ShellVM : BaseVM
   {

      public ShellVM()
      {
         this.NewLibraryCommand = new Command(() => this.NewLibrary());
      }

      public Command NewLibraryCommand { get; set; }
      private void NewLibrary()
      {
         this.IsBusy = true;
         try
         {
            var libraryStore = DependencyService.Get<Libraries.LibraryStore>();
            var library = new Libraries.LibraryModel
            {
               Key = DateTime.Now.Ticks.ToString(),
               Description = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };
            libraryStore.AddLibrary(library);
         }
         catch (Exception) { throw; }
         this.IsBusy = false;
      }

   }
}
