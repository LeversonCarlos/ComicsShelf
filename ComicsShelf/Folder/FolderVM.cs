﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Folder
{
   public class FolderVM : Helpers.ViewModels.DataVM<FolderData>
   {

      #region New
      public FolderVM(FolderData args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(FolderPage);

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
         {
            var folderItem = (FolderData)item;
            await PushAsync<FolderVM>(folderItem);
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region FileTapped
      public Command FileTappedCommand { get; set; }
      private async Task FileTapped(object item)
      {
         try
         {
            var fileItem = (File.FileData)item;
            await PushAsync<File.FileSplashVM>(fileItem);
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region OnSizeChanged
      private enum ScrennOrientationEnum : short { Portrait, Landscape };
      private void OnSizeChanged(object sender, EventArgs e)
      {
         var screenOrientation = (this.ScreenSize.Width > this.ScreenSize.Height ? ScrennOrientationEnum.Landscape : ScrennOrientationEnum.Portrait);

         if (Device.Idiom == TargetIdiom.Phone)
         { this.Data.FileColumns = (screenOrientation == ScrennOrientationEnum.Portrait ? 3 : 5); }
         else if (Device.Idiom == TargetIdiom.Tablet)
         { this.Data.FileColumns = (screenOrientation == ScrennOrientationEnum.Portrait ? 5 : 7); }
         else if (Device.Idiom == TargetIdiom.Desktop)
         { this.Data.FileColumns = (int)Math.Ceiling(this.ScreenSize.Width / (double)100); }
         else { this.Data.FileColumns = 5; }

         this.Data.FileHeightRequest = this.ScreenSize.Width / (double)this.Data.FileColumns * (double)1.30;
      }
      #endregion

   }
}