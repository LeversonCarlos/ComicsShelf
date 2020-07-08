using Xamarin.Forms;

namespace ComicsShelf.Screens.Home
{
   public class Extentions
   {

      internal static void Initialize() => DependencyService.Register<HomeVM>();

      internal static HomeVM Instance => DependencyService.Get<HomeVM>();

      // public static Type GetPageType() => typeof(HomePage);

   }
}
