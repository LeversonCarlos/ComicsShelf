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
         Notifyers.Notify.TimerTicks(now => ChangePosition());
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
         int newPosition = -1;
         try
         {
            newPosition = (Position + 1) % PositionCount;
            Position = newPosition;
         }
         catch (Exception ex)
         {
            var msg = "" +
               $"Error trying to set cover slide position. " +
               $"{Environment.NewLine}Position Count:{PositionCount}" +
               $"{Environment.NewLine}Current Position:{Position}" +
               $"{Environment.NewLine}New Position:{newPosition}" +
               "";
            Helpers.Insights.TrackException(new Exception(msg, ex));
         }
      }

      public void Dispose()
      {
         Notifyers.Notify.TimerTicksUnsubscribe();
      }

   }
}
