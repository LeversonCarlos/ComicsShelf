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
               this.LastPosition = this.Position;
               this.SetPositionVisibility((this.Position - 2), false);
               this.SetPositionVisibility((this.Position - 1), true);
               this.SetPositionVisibility((this.Position ), true);
               this.SetPositionVisibility((this.Position + 1), true);
               this.SetPositionVisibility((this.Position + 2), false);
            }
         }
         catch { }
      }

      void SetPositionVisibility(int position, bool visibility)
      {
         if (position >= 0 && position < this.ItemsSource.GetCount())
         {
            var positionData = (File.FilePageData)this.ItemsSource.GetItem(position);
            if (positionData.IsVisible != visibility)
            { positionData.IsVisible = visibility; }
         }
      }

   }
}