using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers.Observables;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   public class LibraryVM : Helpers.BaseVM
   {

      internal readonly LibraryModel Library;
      public ObservableList<ComicFolderVM> ComicFolders { get; private set; }
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
         this.ComicFolders = new ObservableList<ComicFolderVM>();
         this.OpenCommand = new Command(async (item) => await this.Open(item));
      }

      public void OnAppearing()
      {
         try
         {
            this.IsBusy = true;
            var service = DependencyService.Get<LibraryService>();
            this.ComicFolders.Clear();
            this.ComicFolders.AddRange(this.GetFolderList(service.ComicFiles[this.Library.ID]));
            Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingList", this.Library.ID, this.OnRefreshingList);
            Messaging.Subscribe<ComicFileVM>("OnRefreshingItem", this.Library.ID, this.OnRefreshingItem);
            this.IsBusy = false;
         }
         catch { }
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
            comicFolder.ComicFiles.AddRange(comicFiles
               .Where(comicFile => comicFile.ComicFile.FolderPath == comicFolder.FolderPath)
               .ToList());
         }
         return comicFolders;
      }

      public void OnRefreshingList(List<ComicFileVM> comicFiles)
      {
         this.ComicFolders.ReplaceRange(this.GetFolderList(comicFiles));
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
         if (string.IsNullOrEmpty(comicFile.CachePath))
         { comicFile.CachePath = this.Library.LibraryKey; }
         else { comicFile.CachePath = string.Empty; }
         // await PushAsync<File.FileVM>((File.FileData)item);
      }

   }
}
