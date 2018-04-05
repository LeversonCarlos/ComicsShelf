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

   }
}