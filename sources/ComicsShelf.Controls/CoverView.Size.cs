using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   partial class CoverView
   {

      public static double DefaultHeight { get; private set; }
      public static double DefaultWidth { get; private set; }

      public static void InitDefaultSize()
      {
         try
         {
            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            var displayWidth = System.Math.Min(displayInfo.Width, displayInfo.Height) / displayInfo.Density;

            var itemsPerLine = (Device.Idiom == TargetIdiom.Phone ? 3 : 5);
            CoverView.DefaultWidth = (int)(displayWidth / itemsPerLine);
            CoverView.DefaultHeight = ((int)CoverView.DefaultWidth * 1.50);
         }
         catch (System.Exception) { throw; }
      }


   }
}
