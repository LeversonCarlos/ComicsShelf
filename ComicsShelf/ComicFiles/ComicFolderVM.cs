using ComicsShelf.Helpers.Observables;

namespace ComicsShelf.ComicFiles
{
   public class ComicFolderVM : ObservableObject
   {

      public ObservableList<ComicFileVM> ComicFiles { get; internal set; }
      public ComicFolderVM()
      {
         this.ComicFiles = new ObservableList<ComicFileVM>();
         this.ComicFiles.CollectionChanged += this.ComicFiles_CollectionChanged;
      }

      private void ComicFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         this.HasComicFiles = this.ComicFiles.Count != 0;
      }

      bool _HasComicFiles;
      public bool HasComicFiles
      {
         get { return this._HasComicFiles; }
         set { this.SetProperty(ref this._HasComicFiles, value); }
      }

      string _FolderPath;
      public string FolderPath
      {
         get { return this._FolderPath; }
         set { this.SetProperty(ref this._FolderPath, value); }
      }

   }
}
