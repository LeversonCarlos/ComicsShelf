using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Home
{
   public class LibraryData : Folder.FolderData
   {

      public LibraryData(Helpers.Database.ComicFolder comicFolder) : base(comicFolder)
      {
         this.FileTappedCommand = new Command(async (item) => await this.FileTapped(item));
         this.FolderTappedCommand = new Command(async (item) => await this.FolderTapped(item));
         Helpers.Controls.Messaging.Subscribe<Size>(Helpers.Controls.Messaging.Keys.ScreenSizeChanged, this.OnScreenSizeChanged);
         this.OnScreenSizeChanged(((Helpers.Controls.NavPage)App.Current.MainPage).ScreenSize);
      }

      public Command FileTappedCommand { get; set; }
      private async Task FileTapped(object item)
      {
         try
         { await Helpers.NavVM.PushAsync<File.FileVM>((File.FileData)item); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      public Command FolderTappedCommand { get; set; }
      private async Task FolderTapped(object item)
      {
         try
         { await Helpers.NavVM.PushAsync<Folder.FolderVM<Folder.FolderData>>((Folder.FolderData)item); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      bool _IsFeaturedPage;
      public bool IsFeaturedPage
      {
         get { return this._IsFeaturedPage; }
         set { this.SetProperty(ref this._IsFeaturedPage, value); }
      }

      double _CoverWidthRequest;
      public double CoverWidthRequest
      {
         get { return this._CoverWidthRequest; }
         set { this.SetProperty(ref this._CoverWidthRequest, value); }
      }

      private void OnScreenSizeChanged(Size screenSize)
      {
         var fileColumns = (int)Math.Ceiling(screenSize.Width / (double)160);
         var frameMargins = (double)((fileColumns + 1) * 10);
         this.CoverWidthRequest = (screenSize.Width - frameMargins) / (double)fileColumns;
      }

   }
}