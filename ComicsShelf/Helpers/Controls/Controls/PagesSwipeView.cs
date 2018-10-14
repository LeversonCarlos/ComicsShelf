using CarouselView.FormsPlugin.Abstractions;

namespace ComicsShelf.Helpers.Controls
{
   public class PagesSwipeView : CarouselViewControl
   {

      public PagesSwipeView()
      {
         this.Orientation = CarouselViewOrientation.Horizontal;
         this.InterPageSpacing = 10;
         this.ShowArrows = true;
         this.LastPosition = -1;
         this.PositionSelected += this.OnPositionSelected;
         this.ArrowsBackgroundColor = Colors.Lighter;
      }

      int LastPosition { get; set; }
      void OnPositionSelected(object sender, PositionSelectedEventArgs e)
      {
         try
         {
            if (this.Position < 0) { return; }
            if (this.Position == this.LastPosition) { return; }
            if (this.Position == 0 && this.LastPosition != 1 && this.LastPosition != -1) {
               if (this.LastPosition != this.ItemsSource.GetCount()) { return; }
            }

            // this.SetPositionVisibility((this.Position - 3), false);
            this.SetPositionVisibility((this.Position - 2), false);
            this.SetPositionVisibility((this.Position - 1), true);
            this.SetPositionVisibility((this.Position), true);
            this.SetPositionVisibility((this.Position + 1), true);
            this.SetPositionVisibility((this.Position + 2), false);
            // this.SetPositionVisibility((this.Position + 3), false);
            this.LastPosition = this.Position;
         }
         catch { }
      }

      void SetPositionVisibility(int position, bool visibility)
      {
         if (position >= 0 && position < this.ItemsSource.GetCount())
         {
            var positionData = (Views.File.PageData)this.ItemsSource.GetItem(position);
            if (positionData.IsVisible != visibility)
            { positionData.IsVisible = visibility; }
         }
      }

   }
}