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
      private void OnSizeChanged(object sender, EventArgs e)
      {
         var fileColumns = (int)Math.Ceiling(this.ScreenSize.Width / (double)130);
         var frameMargins = (double)((fileColumns + 1) * 10);
         this.CoverWidthRequest = (this.ScreenSize.Width - frameMargins) / (double)fileColumns;
      }
      #endregion

   }
}