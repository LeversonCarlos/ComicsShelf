using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class Modal
   {

      private static Page MainPage => Application.Current.MainPage;

      private static INavigation Navigation => MainPage.Navigation;

      public static Task Push(Page page) => Navigation.PushModalAsync(page, true);

      public static Task Pop() => Navigation.PopModalAsync(true);

   }
}
