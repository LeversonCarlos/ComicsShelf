using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverSize
   {

      public static double DefaultHeight { get; private set; }
      public static double DefaultWidth { get; private set; }

      public static void Init()
      {
         try
         {
            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            var displayWidth = System.Math.Min(displayInfo.Width, displayInfo.Height) / displayInfo.Density;

            var itemsPerLine = (Device.Idiom == TargetIdiom.Phone ? 3 : 5);
            CoverSize.DefaultWidth = (int)(displayWidth / itemsPerLine);
            CoverSize.DefaultHeight = ((int)CoverSize.DefaultWidth * 1.50);
         }
         catch (System.Exception) { throw; }
      }


   }
}
