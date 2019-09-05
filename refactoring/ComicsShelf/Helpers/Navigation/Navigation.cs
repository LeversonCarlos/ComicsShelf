using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   partial class App
   {

      public static async Task NavigationPopToRootAsync()
      {
         var mainPage = Application.Current.MainPage as Main.MainPage;
         var navigationPage = mainPage.Detail as NavigationPage;
         await navigationPage.Navigation.PopToRootAsync();
      }

      public static async Task NavigationPushAsync(Page page)
      {
         var mainPage = Application.Current.MainPage as Main.MainPage;
         var navigationPage = mainPage.Detail as NavigationPage;
         await navigationPage.Navigation.PopToRootAsync();
         await navigationPage.PushAsync(page);
      }

   }
}
