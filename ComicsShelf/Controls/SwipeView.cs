using CarouselView.FormsPlugin.Abstractions;
using System;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class SwipeView : CarouselViewControl, IDisposable
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
         this.ArrowsBackgroundColor = Helpers.Colors.Accent;

         this.GestureRecognizers.Add(new TapGestureRecognizer
         {
            Command = new Command((param) =>
            { Messaging.Send(StatsView.Messaging_ShowStatsView); }),
            NumberOfTapsRequired = 1
         });
         Helpers.AppCenter.TrackEvent("SwipeView.OnInitialize");
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
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
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

      public void Dispose()
      {
         try
         {
            Helpers.AppCenter.TrackEvent("SwipeView.OnDispose");
            this.GestureRecognizers.Clear();
            if (this.ItemsSource != null)
            {
               var comicFileVM = (this.ItemsSource as ComicFiles.ComicFileVM);
               foreach (var page in comicFileVM.Pages) { page.IsVisible = false; }
               comicFileVM.Pages.Clear();
            }
            this.ItemsSource = null;
         }
         catch(Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

   }
}