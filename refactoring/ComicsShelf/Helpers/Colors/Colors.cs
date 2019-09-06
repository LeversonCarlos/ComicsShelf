using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class Colors
   {
      public static Color Primary { get { return (Color)Application.Current.Resources["colorPrimary"]; } }
      public static Color Accent { get { return (Color)Application.Current.Resources["colorAccent"]; } }
   }
}
