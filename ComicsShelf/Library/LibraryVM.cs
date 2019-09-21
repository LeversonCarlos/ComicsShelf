using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Library
{
   public class LibraryVM : BaseVM, IDisposable
   {

      public readonly Store.LibraryModel Library;
      public ObservableList<ComicFolderVM> ComicFolders { get; private set; }
      public Notify.NotifyVM Notify { get; private set; }

      public LibraryVM(Store.LibraryModel library)
      {
         this.Library = library;
         this.Title = library.Description;
         this.Notify = new Notify.NotifyVM(library.ID);
         this.ComicFolders = new ObservableList<ComicFolderVM>();
         this.OpenCommand = new Command(async (item) => await this.Open(item as ComicFileVM));
         this.RemoveCommand = new Command(async () => await this.Remove());
         this.OnRefreshingList(DependencyService.Get<Store.ILibraryStore>().GetLibraryFiles(this.Library));
         Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingList", this.Library.ID, this.OnRefreshingList);
         Messaging.Subscribe<ComicFileVM>("OnRefreshingItem", this.Library.ID, this.OnRefreshingItem);
      }

      bool _HasComicFiles;
      public bool HasComicFiles
      {
         get { return this._HasComicFiles; }
         set { this.SetProperty(ref this._HasComicFiles, value); }
      }

      public Command OpenCommand { get; set; }
      private async Task Open(ComicFileVM item)
      {
         try
         {
            var vm = new Splash.SplashVM(item);
            var page = new Splash.SplashPage { BindingContext = vm };
            await App.Navigation().PushAsync(page);
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      public Command RemoveCommand { get; set; }
      private async Task Remove()
      {
         if (!await App.ConfirmMessage(R.Strings.LIBRARY_REMOVE_CONFIRM_MESSAGE)) { return; }
         await DependencyService.Get<Store.ILibraryStore>().DeleteLibraryAsync(this.Library.ID);
      }

      public void OnRefreshingList(List<ComicFileVM> comicFiles)
      {
         this.ComicFolders.ReplaceRange(this.GetFolderList(comicFiles));
         this.HasComicFiles = this.ComicFolders.SelectMany(x => x.ComicFiles).Count() > 0;
      }

      public void OnRefreshingItem(ComicFileVM comicFile)
      {
         var comicFolder = this.ComicFolders
            .Where(x => x.FolderPath == comicFile.ComicFile.FolderPath)
            .FirstOrDefault();
         if (comicFolder == null) { return; }
         comicFolder.ComicFiles.Replace(comicFile);
      }

      private List<ComicFolderVM> GetFolderList(List<ComicFileVM> comicFiles)
      {
         var comicFolders = comicFiles
            .GroupBy(x => new { x.ComicFile.FolderPath })
            .Select(x => new ComicFolderVM { FolderPath = x.Key.FolderPath })
            .OrderBy(x => x.FolderPath)
            .ToList();
         foreach (var comicFolder in comicFolders)
         {
            var comicFolderFiles = comicFiles
               .Where(comicFile => comicFile.ComicFile.FolderPath == comicFolder.FolderPath)
               .OrderByDescending(comicFile => comicFile.ComicFile.FilePath)
               .ToList();
            comicFolder.FolderPath = comicFolder.FolderPath
               .Replace("/", " -> ")
               .Replace("\\", " -> ");
            comicFolder.ComicFiles.AddRange(comicFolderFiles);
         }
         return comicFolders;
      }

      public void Dispose()
      {
         Messaging.Unsubscribe<List<ComicFileVM>>("OnRefreshingList", this.Library.ID);
         Messaging.Unsubscribe<ComicFileVM>("OnRefreshingItem", this.Library.ID);
         this.ComicFolders.Clear();
      }

   }
}
