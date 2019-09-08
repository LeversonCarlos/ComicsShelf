using System;
using System.Linq;

namespace ComicsShelf
{
   public partial class AppShell : Xamarin.Forms.Shell, IDisposable
   {

      public AppShell()
      {
         InitializeComponent();
         this.BindingContext = new ShellVM();
         Xamarin.Forms.Routing.RegisterRoute("splash", typeof(ComicFiles.SplashView));
         Xamarin.Forms.Routing.RegisterRoute("reading", typeof(ComicFiles.ReadingView));
         this.SizeChanged += this.MainPage_SizeChanged;
      }

      private void MainPage_SizeChanged(object sender, System.EventArgs e)
      {
         var pageSize = new ComicFiles.ComicPageSize(this.Width, this.Height);
         Messaging.Send(ComicFiles.ComicPageSize.PageSizeChanged, pageSize);
      }

      protected override bool OnBackButtonPressed()
      {

         var segments = this.CurrentState.Location
            .ToString()
            .Replace("//", "/")
            .Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
         if (segments.Contains("splash") || segments.Contains("splash"))
         { return base.OnBackButtonPressed(); }

         var route = segments[0];
         if (route == "home")
         { return base.OnBackButtonPressed(); }

         this.CurrentItem = this.Items[0];
         return true; // PREVENT THE BACK BUTON
      }

      public void Dispose()
      {
         this.SizeChanged -= this.MainPage_SizeChanged;
      }

   }
}
