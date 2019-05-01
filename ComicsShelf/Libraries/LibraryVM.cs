using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers.Observables;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries
{
   public class LibraryVM : Helpers.BaseVM
   {

      internal readonly LibraryModel Library;
      public ObservableList<ComicFile> Data { get; private set; }
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
         this.Data = new ObservableList<ComicFile>();
      }


      public async Task LoadLibraryData()
      {
         Messaging.Subscribe<ComicFile[]>("LoadLibraryData", this.Library.LibraryKey, async (files) =>
         {
            this.Data.ReplaceRange(files);
         });
         await Task.Factory.StartNew(() => LibraryEngine.LoadData(this.Library), TaskCreationOptions.LongRunning);
      }

   }
}
