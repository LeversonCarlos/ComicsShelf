using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public partial class CoverContainer : AbsoluteLayout
   {

      public CoverContainer()
      {
         Margin = 0;
         Padding = new Thickness(0, 0, 10, 0);
         WidthRequest = CoverView.DefaultWidth; ;
         HeightRequest = CoverView.DefaultHeight;
      }

   }
}
