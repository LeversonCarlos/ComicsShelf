using ComicsShelf.Controls;
using System;

namespace ComicsShelf.Splash
{
   public class SplashCover
   {

      public static double DefaultHeight => Math.Round(CoverSize.DisplayWidth * 1.0, 0);
      public static double DefaultWidth => Math.Round(DefaultHeight / 1.5, 0);

   }
}
