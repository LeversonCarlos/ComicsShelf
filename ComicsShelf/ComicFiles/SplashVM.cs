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
            .OrderByDescending(x => x.ComicFile.FilePath)
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
         set
         {
            this.ComicFiles.ForEach(x => x.Readed = true);
            this.SetProperty(ref this._IsAllReaded, value);
         }
      }

      ComicPageSize _PageSize;
      public ComicPageSize PageSize
      {
         get { return this._PageSize; }
         set { this.SetProperty(ref this._PageSize, value); }
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
            if (!await App.ConfirmMessage(R.Strings.SPLASH_FILE_CLEAR_COMIC_CACHE_MESSAGE)) { return; }

            if (System.IO.Directory.Exists(this.CurrentFile.CachePath))
            { System.IO.Directory.Delete(this.CurrentFile.CachePath, true); }
            if (System.IO.File.Exists($"{this.CurrentFile.CachePath}.zip"))
            { System.IO.File.Delete($"{this.CurrentFile.CachePath}.zip"); }

            this.CurrentFile.CachePath = string.Empty;
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      public Command ItemOpenCommand { get; set; }
      private async Task ItemOpen()
      {
         try
         {
            this.IsBusy = true;
            Messaging.Send("OnComicFileOpening", this.CurrentFile);
            var startTime = DateTime.Now;

            var libraryService = DependencyService.Get<Libraries.LibraryService>();
            var library = libraryService.Libraries[this.CurrentFile.ComicFile.LibraryKey];
            if (library == null) { return; }
            var engine = Engines.Engine.Get(library.Type);
            if (engine == null) { return; }

            var wasCached = System.IO.Directory.Exists(this.CurrentFile.ComicFile.CachePath);
            var pages = await engine.ExtractPages(library, this.CurrentFile.ComicFile);
            if (System.IO.Directory.Exists(this.CurrentFile.ComicFile.CachePath))
            {
               if (pages != null && pages.Count != 0)
               {
                  this.CurrentFile.CachePath = this.CurrentFile.ComicFile.CachePath;
                  this.CurrentFile.Pages = new Helpers.Observables.ObservableList<ComicPageVM>(pages.ToList());
                  this.CurrentFile.Pages.Add(new ComicPageVM { Text = "END" });
                  this.CurrentFile.ComicFile.TotalPages = (short)(this.CurrentFile.Pages.Count - 2); /* cover and the end one */
                  await Shell.Current.GoToAsync($"reading?libraryID={this.CurrentFile.ComicFile.LibraryKey}&comicKey={this.CurrentFile.ComicFile.Key}");
               }
            }

            var endTime = DateTime.Now;
            var trackProps = new Dictionary<string, string> {
               { "ElapsedSeconds", ((int)(endTime-startTime).TotalSeconds).ToString() },
               { "WasCached", wasCached.ToString() }
            };
            Helpers.AppCenter.TrackEvent($"Comic.{library.Type.ToString()}.Open", trackProps);


            Messaging.Send("OnComicFileOpened", this.CurrentFile);
            this.IsBusy = false;
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

   }
}
