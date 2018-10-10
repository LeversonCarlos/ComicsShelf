using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Folder
{
   public class FolderVM<T> : Helpers.DataVM<T> where T:Folder.FolderData
   {

      #region New
      public FolderVM(T args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(FolderView);

         this.Data = args;
         this.FolderTappedCommand = new Command(async (item) => await this.FolderTapped(item));
         this.FileTappedCommand = new Command(async (item) => await this.FileTapped(item));
         this.SizeChanged += this.OnSizeChanged;
      }
      #endregion


      #region FolderTapped
      public Command FolderTappedCommand { get; set; }
      private async Task FolderTapped(object item)
      {
         try
         { await PushAsync<FolderVM<Folder.FolderData>>((Folder.FolderData)item); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region FileTapped
      public Command FileTappedCommand { get; set; }
      private async Task FileTapped(object item)
      {
         try
         { await PushAsync<File.FileVM>((File.FileData)item); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

      #region CoverWidthRequest
      double _CoverWidthRequest;
      public double CoverWidthRequest
      {
         get { return this._CoverWidthRequest; }
         set { this.SetProperty(ref this._CoverWidthRequest, value); }
      }
      #endregion

      #region OnSizeChanged
      // private enum ScreenOrientation : short { Portrait, Landscape };
      private void OnSizeChanged(object sender, EventArgs e)
      {
         // var screenOrientation = (this.ScreenSize.Width > this.ScreenSize.Height ? ScreenOrientation.Landscape : ScreenOrientation.Portrait);
         var fileColumns = (int)Math.Ceiling(this.ScreenSize.Width / (double)120);

         //if (Device.Idiom == TargetIdiom.Phone)
         //{ fileColumns = (screenOrientation == ScreenOrientation.Portrait ? 3 : 5); }
         //else if (Device.Idiom == TargetIdiom.Tablet)
         //{ fileColumns = (screenOrientation == ScreenOrientation.Portrait ? 5 : 7); }

         this.CoverWidthRequest = (int)this.ScreenSize.Width / fileColumns;
      }
      #endregion

   }
}