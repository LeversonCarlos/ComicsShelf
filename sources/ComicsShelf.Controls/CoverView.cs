using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public partial class CoverView : AbsoluteLayout
   {

      public CoverView()
      {
         Margin = 0;
         Padding = new Thickness(0, 0, 10, 0);
         WidthRequest = CoverView.DefaultWidth; ;
         HeightRequest = CoverView.DefaultHeight;
      }

   }
}
