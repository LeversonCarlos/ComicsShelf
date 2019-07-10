using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   public class LibraryVM : Helpers.BaseVM
   {

      public Notify.NotifyVM Notify { get; private set; }
      internal readonly LibraryModel Library;
      public ObservableList<ComicFolderVM> ComicFolders { get; private set; }
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
         this.Notify = new Notify.NotifyVM(value.LibraryKey);
         this.ComicFolders = new ObservableList<ComicFolderVM>();
         this.OpenCommand = new Command(async (item) => await this.Open(item));
      }

      public async void OnAppearing()
      {
         try
         {
            this.IsBusy = true;
            var service = DependencyService.Get<LibraryService>();
            this.ComicFolders.Clear();
            var fileList = service.ComicFiles[this.Library.ID];
            var folderList = this.GetFolderList(fileList);
            this.ComicFolders.ReplaceRange(folderList);
            this.HasComicFiles = this.ComicFolders.SelectMany(x => x.ComicFiles).Count() > 0;
            Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingList", this.Library.ID, this.OnRefreshingList);
            Messaging.Subscribe<ComicFileVM>("OnRefreshingItem", this.Library.ID, this.OnRefreshingItem);
            this.IsBusy = false;
         }
         catch (Exception ex) { await App.ShowMessage(ex); }
      }

      public void OnDisappearing()
      {
         try
         {
            Messaging.Unsubscribe("OnRefreshingList", this.Library.ID);
            Messaging.Unsubscribe("OnRefreshingItem", this.Library.ID);
            this.ComicFolders.Clear();
         }
         catch { }
      }

      private List<ComicFolderVM> GetFolderList(List<ComicFileVM> comicFiles)
      {
         var comicFolders = comicFiles
            .GroupBy(x => new { x.ComicFile.FolderPath })
            .Select(x => new ComicFolderVM
            {
               FolderPath = x.Key.FolderPath
            })
            .ToList();
         foreach (var comicFolder in comicFolders)
         {
            var comicFolderFiles = comicFiles
               .Where(comicFile => comicFile.ComicFile.FolderPath == comicFolder.FolderPath)
               .ToList();
            // comicFolderFiles = comicFolderFiles.Take(1).ToList();
            comicFolder.ComicFiles.AddRange(comicFolderFiles);
         }
         return comicFolders;
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

      public Command OpenCommand { get; set; }
      private async Task Open(object item)
      {
         var comicFile = item as ComicFileVM;
         await Shell.Current.GoToAsync($"splash?libraryID={comicFile.ComicFile.LibraryKey}&comicKey={comicFile.ComicFile.Key}");
      }

      bool _HasComicFiles;
      public bool HasComicFiles
      {
         get { return this._HasComicFiles; }
         set { this.SetProperty(ref this._HasComicFiles, value); }
      }

   }
}
