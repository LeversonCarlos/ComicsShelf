using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class CoverOpening : TriggerAction<CoverImage>
   {

      public enum AnimationAction { Start, Stop }
      public AnimationAction Action { get; set; }

      protected override void Invoke(CoverImage sender)
      {
         if (sender != null)
         {
            if (Action == AnimationAction.Start)
               Start(sender);
            else if (Action == AnimationAction.Stop)
               Stop(sender);
         }
      }

      Task<bool[]> Start(CoverImage sender) =>
         Task.WhenAll(
           sender.FadeTo(0.05, 50000, Easing.SinInOut),
           sender.ScaleTo(20, 50000, Easing.SinInOut)
        );


      void Stop(CoverImage sender)
      {
         ViewExtensions.CancelAnimations(sender);
         Task.WhenAll(
            sender.FadeTo(1, 100, Easing.SinInOut),
            sender.ScaleTo(1, 100, Easing.SinInOut)
         );
      }
   }
}
