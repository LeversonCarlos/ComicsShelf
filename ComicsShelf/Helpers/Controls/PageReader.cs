using CarouselView.FormsPlugin.Abstractions;

namespace ComicsShelf.Helpers.Controls
{
   public class PageReader : CarouselViewControl
   {
      public PageReader()
      {
         this.Orientation = CarouselViewOrientation.Horizontal;
         this.InterPageSpacing = 10;
         this.ShowArrows = true;
         this.IsSwipeEnabled = true;
      }          
   }
}