using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Animations
{
   public class OpacityAnimation : TriggerAction<View>
   {

      public double Opacity { get; set; }
      public double? Speed { get; set; }
      uint Duration => (uint)Math.Round(100 / (Speed ?? 1.0), 0);

      protected override async void Invoke(View sender)
      {
         if (sender != null)
            await Animate(sender);
      }

      async Task Animate(View sender)
      {
         // ViewExtensions.CancelAnimations(sender);
         await sender.FadeTo(Opacity, Duration, Easing.CubicOut);
      }

   }
}
