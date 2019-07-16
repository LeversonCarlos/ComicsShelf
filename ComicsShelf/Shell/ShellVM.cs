using ComicsShelf.Helpers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   public class ShellVM : BaseVM
   {

      public ShellVM()
      {
         this.NewLocalLibraryCommand = new Command(async () => await this.NewLibrary(Libraries.LibraryType.LocalDrive));
         this.NewOneDriveLibraryCommand = new Command(async () => await this.NewLibrary(Libraries.LibraryType.OneDrive));
         this.DeleteLibraryCommand = new Command(async (shellItem) => await this.DeleteLibrary((ShellItem)shellItem));
      }

      public Command NewLocalLibraryCommand { get; set; }
      public Command NewOneDriveLibraryCommand { get; set; }
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
