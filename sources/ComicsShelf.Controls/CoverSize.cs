using System;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverSize
   {

      public static double DisplayWidth { get; private set; }

      public static double DefaultHeight { get; private set; }
      public static double DefaultWidth { get; private set; }

      public static void Init()
      {
         try
         {
            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            DisplayWidth = Math.Min(displayInfo.Width, displayInfo.Height) / displayInfo.Density;

            var itemMargin = 5;
            var displayMargin = 10;
            DisplayWidth -= (displayMargin * 2);
            var itemsPerLine = (Device.Idiom == TargetIdiom.Phone ? 3 : 5);

            CoverSize.DefaultWidth = Math.Floor(DisplayWidth / itemsPerLine) - (itemMargin * 2);
            CoverSize.DefaultHeight = ((int)CoverSize.DefaultWidth * 1.50);
         }
         catch (System.Exception) { throw; }
      }


   }
}
