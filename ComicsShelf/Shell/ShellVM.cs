using ComicsShelf.Helpers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   public class ShellVM : BaseVM
   {

      public ShellVM()
      {
         this.NewLibraryCommand = new Command(async (libraryType) => await this.NewLibrary((Libraries.LibraryType)libraryType));
         this.DeleteLibraryCommand = new Command(async (shellItem) => await this.DeleteLibrary((ShellItem)shellItem));
      }

      public Command NewLibraryCommand { get; set; }
      private async Task NewLibrary(Libraries.LibraryType libraryType)
      {
         this.IsBusy = true;
         var libraryStore = DependencyService.Get<Libraries.LibraryStore>();
         await libraryStore.NewLibrary(libraryType);
         this.IsBusy = false;
      }

      public Command DeleteLibraryCommand { get; set; }
      private async Task DeleteLibrary(ShellItem shellItem)
      {
         if (!await App.ConfirmMessage(R.Strings.LIBRARY_REMOVE_CONFIRM_MESSAGE)) { return; }
         this.IsBusy = true;
         var libraryStore = DependencyService.Get<Libraries.LibraryStore>();
         await libraryStore.DeleteLibrary(shellItem);
         this.IsBusy = false;
      }


   }
}
