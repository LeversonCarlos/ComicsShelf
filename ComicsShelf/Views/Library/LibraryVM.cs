using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Views.Library
{
   public class LibraryVM : Helpers.DataVM<LibraryData>
   {

      #region New
      public LibraryVM()
      {
         this.Title = R.Strings.LIBRARY_MAIN_TITLE; ;
         this.ViewType = typeof(LibraryView);
         this.Data = new LibraryData();
         this.AddLibraryTappedCommand = new Command(async (item) => await this.AddLibraryTapped(item));
         this.RemoveLibraryTappedCommand = new Command(async (item) => await this.RemoveLibraryTapped(item));
         this.AddOneDriveLibraryCommand = new Command(async (item) => await this.AddOneDriveLibrary(item));
         this.LinkTappedCommand = new Command(async (item) => await this.LinkTapped(item));
         Engine.AppCenter.TrackEvent("Library: Show View");
      }
      #endregion

      #region AddLibraryTapped
      public Command AddLibraryTappedCommand { get; set; }
      private async Task AddLibraryTapped(object item)
      {
         this.IsBusy = true;
         await this.Data.AddLibrary(ComicsShelf.Library.LibraryTypeEnum.FileSystem);
         this.IsBusy = false;
      }
      #endregion

      #region AddOneDriveLibrary
      public Command AddOneDriveLibraryCommand { get; set; }
      private async Task AddOneDriveLibrary(object item)
      {
         this.IsBusy = true;
         await this.Data.AddLibrary(ComicsShelf.Library.LibraryTypeEnum.OneDrive);
         this.IsBusy = false;
      }
      #endregion

      #region RemoveLibraryTapped
      public Command RemoveLibraryTappedCommand { get; set; }
      private async Task RemoveLibraryTapped(object item)
      {
         this.IsBusy = true;
         await this.Data.RemoveLibrary(item as LibraryDataItem);
         this.IsBusy = false;
      }
      #endregion

      #region LinkTapped
      public Command LinkTappedCommand { get; set; }
      private async Task LinkTapped(object item)
      {
         try
         { Device.OpenUri(new Uri("http://www.comicbookplus.com")); }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }
      #endregion

   }
}