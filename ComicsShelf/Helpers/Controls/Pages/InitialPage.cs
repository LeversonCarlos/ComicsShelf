using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{

   public class NavPage : NavigationPage
   {
      public NavPage() : base(new InitialPage())
      {
         this.BarTextColor = Color.White;
         this.BarBackgroundColor = Colors.Primary;
         this.SizeChanged += this.OnSizeChanged;
      }
      internal Size ScreenSize { get; set; }
      private void OnSizeChanged(object sender, System.EventArgs e) {
         if (this.ScreenSize != Size.Zero &&
             this.ScreenSize.Width == this.Width &&
             this.ScreenSize.Height == this.Height)
         { return; };
         this.ScreenSize = new Size(this.Width, this.Height);
         Messaging.Send(Messaging.Keys.ScreenSizeChanged, this.ScreenSize);
      }
   }

   public class InitialPage : ContentPage
   {
      public InitialPage()
      {
         this.Title = "Comics Shelf";
         this.Content = new ActivityIndicator
         {
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            VerticalOptions = LayoutOptions.StartAndExpand
         };
      }
   }

}