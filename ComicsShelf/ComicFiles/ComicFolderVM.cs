using ComicsShelf.Helpers.Observables;

namespace ComicsShelf.ComicFiles
{
   public class ComicFolderVM : ObservableObject
   {

      public ObservableList<ComicFileVM> ComicFiles { get; internal set; }
      public ComicFolderVM()
      {
         this.ComicFiles = new ObservableList<ComicFileVM>();
      }

      string _FolderPath;
      public string FolderPath
      {
         get { return this._FolderPath; }
         set { this.SetProperty(ref this._FolderPath, value); }
      }

   }
}
