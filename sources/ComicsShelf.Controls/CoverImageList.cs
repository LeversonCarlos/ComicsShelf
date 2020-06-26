using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverImageList : CarouselView, IDisposable
   {

      public CoverImageList()
      {
         ItemTemplate = GetItemTemplate();
         Helpers.Notify.CoverSliderTimer(now => ChangePosition());
      }

      DataTemplate GetItemTemplate() => new DataTemplate(() =>
      {
         var image = new Image { };
         image.SetBinding(Image.SourceProperty, "CoverPath");

         var imageGrid = new Grid { Children = { image } };
         AbsoluteLayout.SetLayoutFlags(imageGrid, AbsoluteLayoutFlags.All);
         AbsoluteLayout.SetLayoutBounds(imageGrid, new Rectangle(0, 0, 1, 1));

         return imageGrid;
      });

      int PositionCount
      {
         get
         {
            if (ItemsSource == null) { return 1; }
            return ((IList<object>)ItemsSource).Count;
         }
      }

      void ChangePosition()
      {
         try
         {
            Position = (Position + 1) % PositionCount;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

      public void Dispose()
      {
         Helpers.Notify.CoverSliderTimerUnsubscribe();
      }

   }
}
