using ComicsShelf.ComicFiles;
using ComicsShelf.Helpers.Observables;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   public class LibraryVM : Helpers.BaseVM
   {

      internal readonly LibraryModel Library;
      public ObservableList<ComicFile> ComicFiles { get; private set; }
      public LibraryVM(LibraryModel value)
      {
         this.Library = value;
         this.Title = value.Description;
         this.ComicFiles = new ObservableList<ComicFile>();
      }

      public async Task OnAppearing()
      {
         this.IsBusy = true;
         var service = DependencyService.Get<LibraryService>();
         this.ComicFiles.AddRange(service.GetLibraryFiles(this.Library));
         this.IsBusy = false;
      }

   }
}
