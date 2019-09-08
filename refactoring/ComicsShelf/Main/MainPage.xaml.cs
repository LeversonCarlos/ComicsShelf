using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Main
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class MainPage : MasterDetailPage, IDisposable
   {
      public MainPage()
      {
         InitializeComponent();
         this.MasterBehavior = MasterBehavior.Popover;
         this.SizeChanged += this.MainPage_SizeChanged;
      }

      private void MainPage_SizeChanged(object sender, System.EventArgs e)
      {
         var pageSize = new ComicFiles.ComicPageSize(this.Width, this.Height);
         Messaging.Send(ComicFiles.ComicPageSize.PageSizeChanged, pageSize);
      }

      public void Dispose()
      {
         this.SizeChanged -= this.MainPage_SizeChanged;
      }

   }
}
