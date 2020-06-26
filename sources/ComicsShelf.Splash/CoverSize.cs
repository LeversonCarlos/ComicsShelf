using System;

namespace ComicsShelf.Splash
{
   public class SplashCover
   {

      public static double DefaultHeight => Math.Round(Helpers.Cover.ScreenWidth * 0.95, 0);
      public static double DefaultWidth => Math.Round(DefaultHeight / 1.5, 0);

   }
}
