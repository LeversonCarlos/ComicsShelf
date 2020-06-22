using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverContainer : AbsoluteLayout
   {

      public CoverContainer()
      {
         Margin = 0;
         Padding = new Thickness(0, 0, 10, 0);
         WidthRequest = CoverSize.DefaultWidth; ;
         HeightRequest = CoverSize.DefaultHeight;
      }

   }
}
