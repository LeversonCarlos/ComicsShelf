using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   partial class App
   {

      public static INavigation Navigation()
      {
         var mainPage = Application.Current.MainPage as Main.MainPage;
         var navigationPage = mainPage.Detail as NavigationPage;
         return navigationPage.Navigation;
      }

      public static void HideNavigation()
      {
         var mainPage = Application.Current.MainPage as Main.MainPage;
         mainPage.IsPresented = false;
      }

   }
}
