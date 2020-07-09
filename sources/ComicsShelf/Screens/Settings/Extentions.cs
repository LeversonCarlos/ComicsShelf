using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   public class Extentions
   {

      internal static void Initialize() => DependencyService.Register<SettingsVM>();

      internal static SettingsVM Instance => DependencyService.Get<SettingsVM>();

      // public static Type GetPageType() => typeof(HomePage);

   }
}
