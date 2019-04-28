using ComicsShelf.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   public class ShellVM : BaseVM
   {

      public ShellVM()
      {
         this.NewLibraryCommand = new Command(async (libraryType) => await this.NewLibrary((Libraries.LibraryType)libraryType));
         this.DeleteLibraryCommand = new Command((shellItem) => this.DeleteLibrary((ShellItem)shellItem));
      }

      public Command NewLibraryCommand { get; set; }
      private async Task NewLibrary(Libraries.LibraryType libraryType)
      {
         this.IsBusy = true;
         try
         {
            var engine = Engines.Engine.Get(libraryType);
            if (engine == null) { return; }

            var library = await engine.NewLibrary();
            if (library == null) { return; }

            var libraryStore = DependencyService.Get<Libraries.LibraryStore>();
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
