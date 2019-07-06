using CarouselView.FormsPlugin.Abstractions;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class SwipeView : CarouselViewControl
   {

      public SwipeView()
      {
         this.VerticalOptions = LayoutOptions.FillAndExpand;
         this.HorizontalOptions = LayoutOptions.FillAndExpand;
         this.Orientation = CarouselViewOrientation.Horizontal;
         this.InterPageSpacing = 10;
         this.ShowArrows = true;
         this.LastPosition = -1;
         this.PositionSelected += this.OnPositionSelected;
         this.ArrowsBackgroundColor = Color.Accent;

         this.GestureRecognizers.Add(new TapGestureRecognizer
         {
            Command = new Command((param) =>
            { Messaging.Send(StatsView.Messaging_ShowStatsView); }),
            NumberOfTapsRequired = 1
         });

      }

      int LastPosition { get; set; }
      void OnPositionSelected(object sender, PositionSelectedEventArgs e)
      {
         try
         {
            if (this.Position < 0) { return; }
            if (this.Position == this.LastPosition) { return; }
            if (this.Position == 0 && this.LastPosition != 1 && this.LastPosition != -1)
            {
               if (this.LastPosition != this.ItemsSource.GetCount()) { return; }
            }

            this.SetPositionVisibility((this.Position - 2), false);
            this.SetPositionVisibility((this.Position - 1), true);
            this.SetPositionVisibility((this.Position), true);
            this.SetPositionVisibility((this.Position + 1), true);
            this.SetPositionVisibility((this.Position + 2), false);

            this.LastPosition = this.Position;
         }
         catch { }
      }

      void SetPositionVisibility(int position, bool visibility)
      {
         if (position >= 0 && position < this.ItemsSource.GetCount())
         {
            var positionData = (ComicFiles.ComicPageVM)this.ItemsSource.GetItem(position);
            if (positionData.IsVisible != visibility)
            { positionData.IsVisible = visibility; }
         }
      }

   }
}