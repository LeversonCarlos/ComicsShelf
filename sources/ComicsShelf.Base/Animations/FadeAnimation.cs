using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Animations
{
   public class FadeAnimation : TriggerAction<View>
   {

      protected override async void Invoke(View sender)
      {
         if (sender != null)
            await Animate(sender);
      }

      async Task Animate(View sender)
      {
         ViewExtensions.CancelAnimations(sender);
         sender.Opacity = 1;
         await Task.Delay(3000);
         await sender.FadeTo(0, 2000, Easing.CubicIn);
      }

   }
}
