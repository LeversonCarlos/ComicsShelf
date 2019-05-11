using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.ComicFiles
{
   public class SplashVM : Helpers.BaseVM
   {

      public List<ComicFileVM> ComicFiles { get; private set; }
      public SplashVM(ComicFileVM currentFile)
      {
         this.CurrentFile = currentFile;
         var libraryService = DependencyService.Get<Libraries.LibraryService>();
         var comicFiles = libraryService
            .ComicFiles[this.CurrentFile.ComicFile.LibraryKey]
            .Where(x => x.ComicFile.Available && x.ComicFile.FolderPath == CurrentFile.ComicFile.FolderPath)
            .OrderBy(x => x.ComicFile.FilePath)
            .ToList();
         this._IsAllReaded = comicFiles.Count(x => !x.Readed) == 0;
         this.ComicFiles = comicFiles;
         this.ItemSelectedCommand = new Command((item) => this.ItemSelected(item));
         this.ItemOpenCommand = new Command(async () => await this.ItemOpen());
         this.ClearCacheCommand = new Command(async () => await this.ClearCache());
      }

      ComicFileVM _CurrentFile;
      public ComicFileVM CurrentFile
      {
         get { return this._CurrentFile; }
         set { this.SetProperty(ref this._CurrentFile, value); }
      }

      bool _IsAllReaded;
      public bool IsAllReaded
      {
         get { return this._IsAllReaded; }
         set {
            this.ComicFiles.ForEach(x => x.Readed = true);
            this.SetProperty(ref this._IsAllReaded, value);
         }
      }

      public Command ItemSelectedCommand { get; set; }
      private void ItemSelected(object item)
      {
         try
         {
            this.CurrentFile = item as ComicFileVM;
         }
         catch (Exception) { throw; }
      }

      public Command ClearCacheCommand { get; set; }
      private async Task ClearCache()
      {
         try
         {
            this.CurrentFile.CacheStatus = CacheStatusEnum.No;
         }
         catch (Exception) { throw; }
      }

      public Command ItemOpenCommand { get; set; }
      private async Task ItemOpen()
      {
         try
         {
            //this.CurrentFile;
            this.IsBusy = true;
         }
         catch (Exception) { throw; }
      }

   }
}
