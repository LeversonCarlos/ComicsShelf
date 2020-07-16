using ComicsShelf.Screens.Splash;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Home
{
   partial class HomeVM
   {

      public Command OpenCommand { get; set; }
      async Task OpenAsync(object folder)
      {
         if (folder == null) { return; }
         var viewModel = await GetSplashVM(folder as FolderVM);
         await Shell.Current.GoToAsync(viewModel);
      }

      Task<SplashVM> GetSplashVM(FolderVM folder) =>
         Task.FromResult(SplashVM.Create(folder?.FirstItem));

   }
}
