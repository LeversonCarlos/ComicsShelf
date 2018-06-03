using CarouselView.FormsPlugin.Abstractions;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class PageReader : CarouselViewControl
   {
      public PageReader()
      {
         this.Orientation = CarouselViewOrientation.Horizontal;
         this.InterPageSpacing = 10;
         this.ShowArrows = true;
         this.LastPosition = -1;
         this.PositionSelected += this.OnPositionSelected;
      }

      int LastPosition { get; set; }
      void OnPositionSelected(object sender, PositionSelectedEventArgs e)
      {
         try
         {
            this.IsSwipeEnabled = true;
            if (this.LastPosition != this.Position)
            {

               if (this.LastPosition != -1 )
               {
                  var lastPositionData = (File.FilePageData)this.ItemsSource.GetItem(this.LastPosition);
                  lastPositionData.IsVisible = false;
               }
               this.LastPosition = this.Position;

               var currentPositionData = (File.FilePageData)this.ItemsSource.GetItem(this.Position);
               currentPositionData.IsVisible = true;
            }
         }
         catch { }
      }

   }
}