using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers;
using ComicsShelf.Helpers.Observables;
using System.Collections.Generic;
using System.Linq;

namespace ComicsShelf.Home
{
   public class HomeVM : BaseVM
   {

      public Notify.NotifyVM Notify { get; private set; }
      public ObservableList<ComicFolderVM> ComicFolders { get; private set; }
      public HomeVM()
      {
         this.Title = "Home";
         this.Notify = new Notify.NotifyVM("LibraryService");
         this.ComicFolders = new ObservableList<ComicFolderVM>();
         this.ComicFolders.Add(new ComicFolderVM { FolderPath = R.Strings.HOME_READING_FILES_SECTION_TITLE });
         this.ComicFolders.Add(new ComicFolderVM { FolderPath = R.Strings.HOME_RECENT_FILES_SECTION_TITLE });
         Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingReadingFilesList", this.OnRefreshingReadingFilesList);
         Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingRecentFilesList", this.OnRefreshingRecentFilesList);
      }

      private void OnRefreshingReadingFilesList(List<ComicFileVM> comicFiles)
      { this.OnRefreshingList(this.ComicFolders[0].ComicFiles, comicFiles); }

      private void OnRefreshingRecentFilesList(List<ComicFileVM> comicFiles)
      { this.OnRefreshingList(this.ComicFolders[1].ComicFiles, comicFiles); }

      private void OnRefreshingList(ObservableList<ComicFileVM> oldItems, List<ComicFileVM> newItems)
      {
         var oldArray = oldItems.Select(x => x.ComicFile.FilePath).ToList();
         var oldText = ""; oldArray.ForEach(x => oldText += $"{x}|");
         var newArray = newItems.Select(x => x.ComicFile.FilePath).ToList();
         var newText = ""; newArray.ForEach(x => newText += $"{x}|");
         if (oldText == newText) { return; }
         oldItems.ReplaceRange(newItems);
      }


   }
}
