namespace ComicsShelf.Helpers.Controls
{

   public class PrimaryButton : Xamarin.Forms.Button
   {
      public PrimaryButton()
      {
         this.BackgroundColor = Colors.Primary;
         this.TextColor = Xamarin.Forms.Color.White;
      }
   }

   public class SecondaryButton : Xamarin.Forms.Button
   {
      public SecondaryButton()
      {
         this.BackgroundColor = Colors.Accent;
         this.TextColor = Xamarin.Forms.Color.White;
      }
   }

}