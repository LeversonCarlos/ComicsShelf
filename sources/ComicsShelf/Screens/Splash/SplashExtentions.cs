using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Splash
{

   partial class SplashVM
   {
      public static SplashVM Create(ItemVM selectedItem)
      {
         try
         {
            var store = DependencyService.Get<IStoreService>();

            var library = store.GetLibrary(selectedItem.LibraryID);
            var libraryItems = store.GetLibraryItems(library);

            var folderItems = libraryItems
               .Where(item => item.FolderPath == selectedItem.FolderPath)
               .OrderBy(item => item.FullText)
               .ToArray();

            return new SplashVM(selectedItem, folderItems);
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); throw; }
      }
   }

   public static class SplashExtentions
   {

      internal static SplashVM Instance { get; set; }

      public static string GetRoute() => "splash";
      public static Type GetPageType() => typeof(SplashPage);

      public static Task GoToAsync(this Shell shell, SplashVM viewModel)
      {
         SplashExtentions.Instance = viewModel;
         var navState = new ShellNavigationState(SplashExtentions.GetRoute());
         return shell.GoToAsync(navState);
      }

   }

}
