using ComicsShelf.Controls;
using System;

namespace ComicsShelf.Splash
{
   public class SplashCover
   {

      public static double DefaultHeight => Math.Round(CoverSize.DisplayWidth * 0.9, 0);
      public static double DefaultWidth => Math.Round(DefaultHeight / 1.5, 0);

   }
}
