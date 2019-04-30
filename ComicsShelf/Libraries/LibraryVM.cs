using ComicsShelf.Helpers;
using ComicsShelf.Helpers.Observables;
using System.Threading.Tasks;

namespace ComicsShelf.Libraries
{
   public class LibraryVM : Helpers.BaseVM
   {

      internal readonly LibraryModel Library;
      public ObservableList<File> Data { get; private set; }
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
         this.Data = new ObservableList<File>();
      }


      public async Task LoadFiles()
      {
         Messaging.Subscribe<File[]>("LoadFiles", this.Library.Key, async (files) =>
         {
            this.Data.ReplaceRange(files);
         });
         await Task.Factory.StartNew(() => LibraryEngine.LoadFiles(this.Library), TaskCreationOptions.LongRunning);
      }

   }
}
