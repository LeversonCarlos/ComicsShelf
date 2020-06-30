using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.Reading
{
   partial class ReadingVM
   {

      public override Task OnAppearing()
      {
         Task.Run(() => InitializeData());
         return base.OnAppearing();
      }

      void InitializeData()
      {
         try
         {
            ItemPages.ReplaceRange(_PagesList);
            ReadingPage = Item.ReadingPage.HasValue ? Item.ReadingPage.Value : (short)0;
         }
         finally { IsBusy = false; }
      }

      /*
      async Task InitializeData()
      {
         var start = DateTime.Now;
         try
         {
            IsBusy = true;
            this.InitializeLibrary();
            this.InitializeItem();
            await this.InitializePages();
         }
         catch (Exception ex) { Helpers.Message.Show(ex); await Helpers.Modal.Pop(); }
         finally { Helpers.Insights.TrackMetric("Reading Initializing", DateTime.Now.Subtract(start).TotalSeconds); IsBusy = false; }
      }
      */

      /*
      string _LibraryID { get; set; }
      public void SetLibraryID(string libraryID) => _LibraryID = System.Uri.UnescapeDataString(libraryID);
      LibraryVM _Library { get; set; }
      void InitializeLibrary()
      {
         if (string.IsNullOrEmpty(_LibraryID)) { throw new Exception($"No libraryID parameter was defined for the loading of the splash screen"); }
         if (_Library != null) { return; }
         _Library = Store.GetLibrary(_LibraryID);
         if (_Library == null) { throw new Exception($"Could not load the library for the libraryID {_LibraryID} on the splash screen"); }
      }
      */

      public ItemVM Item { get; set; }
      /*
      string _FileID { get; set; }
      public void SetFileID(string fileID) => _FileID = fileID;
      void InitializeItem()
      {
         if (string.IsNullOrEmpty(_FileID)) { throw new Exception($"No fileID parameter was defined for the loading of the splash screen"); }
         Item = Store
            .GetLibraryItems(_Library)
            .Where(item => item.EscapedID == _FileID)
            .FirstOrDefault();
         if (Item == null) { throw new Exception($"Could not locate the item {_FileID} for the libraryID {_LibraryID} on the splash screen"); }
      }
      */

      public Observables.ObservableList<PageVM> ItemPages { get; set; } = new Observables.ObservableList<PageVM>();
      /*
      async Task InitializePages()
      {
         var pagesList = await Engine.PagesExtraction.Service.GetPagesAsync(Item);
         pagesList = pagesList.Union(new PageVM[] { new PageVM { } }).ToArray();
         ItemPages.ReplaceRange(pagesList);

         ReadingPage = Item.ReadingPage.HasValue ? Item.ReadingPage.Value : (short)0;
      }
      */

   }
}
