using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class App
   {

      public static Page MainPage => Application.Current.MainPage;

      // public static NavigationPage NavigationPage => MainPage as NavigationPage;

      public static INavigation Navigation => MainPage.Navigation;

      // public static void HideNavigation() => MainPage.IsPresented = false;

   }
}
