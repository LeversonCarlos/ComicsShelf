namespace ComicsShelf.Folder
{
   public class FolderData : Helpers.Observables.ObservableObject
   {

      #region New
      public FolderData()
      {
         this.Folders = new Helpers.Observables.ObservableList<FolderData>();
         this.Files = new Helpers.Observables.ObservableList<File.FileData>();
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

      #region PersistentData
      Database.ComicFolders _PersistentData;
      public Database.ComicFolders PersistentData
      {
         get { return this._PersistentData; }
         set
         { this.SetProperty(ref this._PersistentData, value); }
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

      int _FolderColumns = 1;
      public int FolderColumns
      {
         get { return this._FolderColumns; }
         set { this.SetProperty(ref this._FolderColumns, value); }
      }

      double _FolderHeightRequest;
      public double FolderHeightRequest
      {
         get { return this._FolderHeightRequest; }
         set { this.SetProperty(ref this._FolderHeightRequest, value); }
      }

      private void Folders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         this.HasFolders = this.Folders.Count != 0;
      }

      #endregion

      #region Files

      public Helpers.Observables.ObservableList<File.FileData> Files { get; set; }

      bool _HasFiles;
      public bool HasFiles
      {
         get { return this._HasFiles; }
         set { this.SetProperty(ref this._HasFiles, value); }
      }

      int _FileColumns = 1;
      public int FileColumns
      {
         get { return this._FileColumns; }
         set { this.SetProperty(ref this._FileColumns, value); }
      }

      double _FileHeightRequest;
      public double FileHeightRequest
      {
         get { return this._FileHeightRequest; }
         set { this.SetProperty(ref this._FileHeightRequest, value); }
      }

      private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         this.HasFiles = this.Files.Count != 0;
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