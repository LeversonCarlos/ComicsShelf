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
         this.ItemSelectedCommand = new Command(async (item) => await this.ItemSelected(item));
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
      private async Task ItemSelected(object item)
      {
         try
         {
            this.CurrentFile = item as ComicFileVM;
         }
         catch (Exception) { throw; }
      }

   }
}
