using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverProgress : ProgressBar
   {

      public CoverProgress()
      {
         HeightRequest = 2;
         WidthRequest = CoverSize.DefaultWidth;

         AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional);
         AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 1, -1, -1));
      }

   }
}
