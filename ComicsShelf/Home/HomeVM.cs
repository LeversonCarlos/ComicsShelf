﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Home
{
   public class HomeVM : Helpers.ViewModels.DataVM<HomeData>
   {

      #region New
      public HomeVM(HomeData args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(HomePage);

         this.Data = args;
         this.OpenLibraryCommand = new Command(async (item) => await this.OpenLibrary(item));
         this.FolderTappedCommand = new Command(async (item) => await this.FolderTapped(item));
         this.FileTappedCommand = new Command(async (item) => await this.FileTapped(item));
         this.SizeChanged += this.OnSizeChanged;

         MessagingCenter.Subscribe<Engine.StepData>(this, Engine.StepData.KEY, (data) =>
         {
            this.Data.StepData.Text = data.Text;
            this.Data.StepData.Details = data.Details;
            this.Data.StepData.Progress = data.Progress;
            this.Data.StepData.IsRunning = data.IsRunning;
         });

         this.Initialize += () =>
         {
            if (!this.Data.StepData.IsRunning) { Engine.Statistics.Execute(); }
         };
      }
      #endregion

      #region OpenLibrary
      public Command OpenLibraryCommand { get; set; }
      private async Task OpenLibrary(object item)
      {
         try
         {
            await Helpers.ViewModels.NavVM.PushAsync<Library.LibraryVM>(false);
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region FolderTapped
      public Command FolderTappedCommand { get; set; }
      private async Task FolderTapped(object item)
      {
         try
         {
            var folderItem = (Folder.FolderData)item;
            await PushAsync<Folder.FolderVM>(folderItem);
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
      private void OnSizeChanged(object sender, EventArgs e) {
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