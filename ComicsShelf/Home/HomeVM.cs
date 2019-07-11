using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers;
using ComicsShelf.Helpers.Observables;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Home
{
   public class HomeVM : BaseVM
   {

      public Notify.NotifyVM Notify { get; private set; }
      public ObservableList<ComicFolderVM> ComicFolders { get; private set; }
      public HomeVM()
      {
         this.Title = "Home";
         this.Notify = new Notify.NotifyVM("General");

         var displayInfo = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
         var displayWidth = displayInfo.Width / displayInfo.Density;
         var itemsPerLine = (Device.Idiom == TargetIdiom.Phone ? 3 : 5);
         var coverWidthRequest = ((int)(displayWidth - 10) / itemsPerLine) - 5;
         this.CoverHeightRequest = ((int)coverWidthRequest * 1.53);

         this.ComicFolders = new ObservableList<ComicFolderVM>();
         this.ComicFolders.Add(new ComicFolderVM { FolderPath = R.Strings.HOME_READING_FILES_SECTION_TITLE });
         this.ComicFolders.Add(new ComicFolderVM { FolderPath = R.Strings.HOME_RECENT_FILES_SECTION_TITLE });
         Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingReadingFilesList", this.OnRefreshingReadingFilesList);
         Messaging.Subscribe<List<ComicFileVM>>("OnRefreshingRecentFilesList", this.OnRefreshingRecentFilesList);
         this.OpenCommand = new Command(async (item) => await this.Open(item));
      }

      private void OnRefreshingReadingFilesList(List<ComicFileVM> comicFiles)
      { this.OnRefreshingList(this.ComicFolders[0].ComicFiles, comicFiles); }

      private void OnRefreshingRecentFilesList(List<ComicFileVM> comicFiles)
      { this.OnRefreshingList(this.ComicFolders[1].ComicFiles, comicFiles); }

      private void OnRefreshingList(ObservableList<ComicFileVM> oldItems, List<ComicFileVM> newItems)
      {
         var oldHash = string.Join("|", oldItems.Select(x => x.ComicFile.Key));
         var newHash = string.Join("|", newItems.Select(x => x.ComicFile.Key));
         if (oldHash == newHash) { return; }
         oldItems.ReplaceRange(newItems);
         this.HasComicFiles = this.ComicFolders.SelectMany(x => x.ComicFiles).Count() > 0;
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

      double _CoverHeightRequest;
      public double CoverHeightRequest
      {
         get { return this._CoverHeightRequest; }
         set { this.SetProperty(ref this._CoverHeightRequest, value); }
      }

   }
}
