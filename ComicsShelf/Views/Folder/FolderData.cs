namespace ComicsShelf.Views.Folder
{
   public class FolderData : Helpers.Observables.ObservableObject
   {

      #region New
      internal Helpers.Database.ComicFolder ComicFolder { get; set; }
      internal FolderData(Helpers.Database.ComicFolder _ComicFolder)
      {
         this.Folders = new Helpers.Observables.ObservableList<FolderData>();
         this.Files = new Helpers.Observables.ObservableList<File.FileData>();

         this.ComicFolder = _ComicFolder;
         this.Text = this.ComicFolder.Text;
         // this.CoverPath = this.ComicFolder.CoverPath;

      }
      #endregion

      #region Text
      string _Text;
      public string Text
      {
         get { return this._Text; }
         set { this.SetProperty(ref this._Text, value); }
      }
      #endregion

      #region CoverPath
      string _CoverPath;
      public string CoverPath
      {
         get { return this._CoverPath; }
         set { this.SetProperty(ref this._CoverPath, value, AlwaysInvokePropertyChanged: true); }
      }
      #endregion

      #region Folders

      public Helpers.Observables.ObservableList<Folder.FolderData> Folders { get; set; }

      bool _HasFolders;
      public bool HasFolders
      {
         get { return this._HasFolders; }
         set { this.SetProperty(ref this._HasFolders, value); }
      }

      /*
      private void Folders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      { this.HasFolders = this.Folders.Count != 0; }
      */

      #endregion

      #region Files

      public Helpers.Observables.ObservableList<File.FileData> Files { get; set; }

      bool _HasFiles;
      public bool HasFiles
      {
         get { return this._HasFiles; }
         set { this.SetProperty(ref this._HasFiles, value); }
      }

      /*
      private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      { this.HasFiles = this.Files.Count != 0; }
      */

      #endregion

      #region ReadingPercent
      double _ReadingPercent;
      public double ReadingPercent
      {
         get { return this._ReadingPercent; }
         set
         {
            this.SetProperty(ref this._ReadingPercent, value);
         }
      }
      #endregion


      #region Available
      bool _Available;
      public bool Available
      {
         get { return this._Available; }
         set { this.SetProperty(ref this._Available, value); }
      }
      #endregion

   }
}