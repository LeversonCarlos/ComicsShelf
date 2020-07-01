using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class Cover
   {

      public static string DefaultCover => $"{Xamarin.Essentials.FileSystem.AppDataDirectory}/DefaultCover.png";

      public static double ScreenWidth { get; private set; }
      public static double ScreenMargin { get; private set; }

      public static double ItemMargin { get; private set; }
      public static int ItemsPerLine { get; private set; }

      public static double DefaultHeight { get; private set; }
      public static double DefaultWidth { get; private set; }

      public static double SplashHeight => Math.Round(ScreenWidth * 0.95, 0);
      public static double SplashWidth => Math.Round(SplashHeight / 1.5, 0);

      public static Task Init() => Task.Factory.StartNew(() => InitAsync(), TaskCreationOptions.LongRunning);

      private static async Task InitAsync()
      {
         CalculateCoverSize();
         await ExtractDefaultCoverAsync();
      }

      private static void CalculateCoverSize()
      {
         try
         {


            var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            ScreenWidth = Math.Min(displayInfo.Width, displayInfo.Height) / displayInfo.Density;
            ScreenMargin = 10;
            ScreenWidth -= (ScreenMargin * 2);

            ItemMargin = 5;
            ItemsPerLine = CalculateCoverSize_GetItemsPerLine();

            DefaultWidth = Math.Floor(ScreenWidth / ItemsPerLine) - (ItemMargin * 2);
            DefaultHeight = ((int)DefaultWidth * 1.50);

         }
         catch (Exception ex) { Insights.TrackException(ex); }
      }

      private static short CalculateCoverSize_GetItemsPerLine()
      {
         switch (Device.Idiom)
         {
            case TargetIdiom.Phone:
               return 3;
            case TargetIdiom.Tablet:
               return 5;
            case TargetIdiom.Desktop:
               return 7;
            case TargetIdiom.TV:
               return 9;
            default:
               return 11;
         }
      }

      private static async Task ExtractDefaultCoverAsync()
      {
         try
         {
            if (System.IO.File.Exists(DefaultCover)) { return; }

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            byte[] bytes = null;
            using (var defaultCover = assembly.GetManifestResourceStream("ComicsShelf.Base.Resources.DefaultCover.png"))
            {
               bytes = new byte[defaultCover.Length];
               await defaultCover.ReadAsync(bytes, 0, bytes.Length);
            }
            if (bytes == null || bytes.Length == 0) { return; }

            using (var streamWriter = new System.IO.FileStream(DefaultCover, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
               await streamWriter.WriteAsync(bytes, 0, bytes.Length);
            }

            assembly = null;
         }
         catch (Exception ex) { Insights.TrackException(ex); }
      }

   }
}
