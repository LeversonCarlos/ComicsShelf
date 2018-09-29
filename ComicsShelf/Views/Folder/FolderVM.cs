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


      #region OnSizeChanged
      private enum ScrennOrientationEnum : short { Portrait, Landscape };
      private void OnSizeChanged(object sender, EventArgs e)
      {
         var screenOrientation = (this.ScreenSize.Width > this.ScreenSize.Height ? ScrennOrientationEnum.Landscape : ScrennOrientationEnum.Portrait);

         if (Device.Idiom == TargetIdiom.Phone)
         {
            this.Data.FileColumns = (screenOrientation == ScrennOrientationEnum.Portrait ? 3 : 5);
            this.Data.FolderColumns = (screenOrientation == ScrennOrientationEnum.Portrait ? 2 : 3);
         }
         else if (Device.Idiom == TargetIdiom.Tablet)
         {
            this.Data.FileColumns = (screenOrientation == ScrennOrientationEnum.Portrait ? 5 : 7);
            this.Data.FolderColumns = (screenOrientation == ScrennOrientationEnum.Portrait ? 3 : 4);
         }
         else if (Device.Idiom == TargetIdiom.Desktop)
         {
            this.Data.FileColumns = (int)Math.Ceiling(this.ScreenSize.Width / (double)100);
            this.Data.FolderColumns = (int)Math.Ceiling(this.ScreenSize.Width / (double)240);
         }
         else
         {
            this.Data.FileColumns = 10;
            this.Data.FolderColumns = 6;
         }

         this.Data.FileHeightRequest = this.ScreenSize.Width / (double)this.Data.FileColumns * (double)1.30;
         this.Data.FolderHeightRequest = this.ScreenSize.Width / (double)this.Data.FolderColumns * (double)0.60;
      }
      #endregion

   }
}