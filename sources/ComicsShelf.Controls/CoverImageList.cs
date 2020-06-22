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
         Device.StartTimer(TimeSpan.FromSeconds(5), ChangePosition);
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

      int _PositionCount;
      int PositionCount
      {
         get
         {
            if (ItemsSource == null) { return 1; }
            if (_PositionCount != 0) { return _PositionCount; }
            _PositionCount = ((IList<object>)ItemsSource).Count;
            return _PositionCount;
         }
      }

      bool ChangePosition()
      {
         Position = (Position + 1) % PositionCount;
         return !_Disposed;
      }

      bool _Disposed = false;
      public void Dispose()
      {
         _Disposed = true;
      }

   }
}
