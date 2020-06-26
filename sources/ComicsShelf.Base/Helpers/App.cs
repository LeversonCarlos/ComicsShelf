using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class App
   {

      public static Page MainPage => Application.Current.MainPage;

      public static INavigation Navigation => MainPage.Navigation;

   }
}
