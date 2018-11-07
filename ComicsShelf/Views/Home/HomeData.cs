using System.Collections.Generic;

namespace ComicsShelf.Views.Home
{
   public class HomeData : Folder.FolderData
   {

      #region New
      public HomeData() : base(new Helpers.Database.ComicFolder { Text = R.Strings.AppTitle })
      {
         this.NoComics = true;

         this.RecentFiles = new Helpers.Observables.ObservableList<File.FileData>();
         this.RecentFiles.ObservableCollectionChanged += this.RecentFiles_CollectionChanged;

         this.ReadingFiles = new Helpers.Observables.ObservableList<File.FileData>();
         this.ReadingFiles.ObservableCollectionChanged += this.ReadingFiles_CollectionChanged;

         this.TopRatedFiles = new Helpers.Observables.ObservableList<File.FileData>();
         this.TopRatedFiles.ObservableCollectionChanged += this.TopRatedFiles_CollectionChanged;

         this.FolderSections = new Helpers.Observables.ObservableList<Folder.FolderData>();
         this.Files.ObservableCollectionChanged += this.Files_CollectionChanged;
      }
      #endregion


      #region RecentFiles

      public Helpers.Observables.ObservableList<File.FileData> RecentFiles { get; set; }
      private void RecentFiles_CollectionChanged(object sender, System.EventArgs e)
      { this.HasRecentFiles = this.RecentFiles.Count != 0; }

      bool _HasRecentFiles;
      public bool HasRecentFiles
      {
         get { return this._HasRecentFiles; }
         set { this.SetProperty(ref this._HasRecentFiles, value); }
      }

      #endregion

      #region ReadingFiles

      public Helpers.Observables.ObservableList<File.FileData> ReadingFiles { get; set; }
      private void ReadingFiles_CollectionChanged(object sender, System.EventArgs e)
      { this.HasReadingFiles = this.ReadingFiles.Count != 0; }

      bool _HasReadingFiles;
      public bool HasReadingFiles
      {
         get { return this._HasReadingFiles; }
         set { this.SetProperty(ref this._HasReadingFiles, value); }
      }

      #endregion

      #region TopRatedFiles

      public Helpers.Observables.ObservableList<File.FileData> TopRatedFiles { get; set; }
      private void TopRatedFiles_CollectionChanged(object sender, System.EventArgs e)
      { this.HasTopRatedFiles = this.TopRatedFiles.Count != 0; }

      bool _HasTopRatedFiles;
      public bool HasTopRatedFiles
      {
         get { return this._HasTopRatedFiles; }
         set { this.SetProperty(ref this._HasTopRatedFiles, value); }
      }

      #endregion

      #region FolderSections

      public Helpers.Observables.ObservableList<Folder.FolderData> FolderSections { get; set; }

      #endregion

      #region CollectionChanged

      private void Files_CollectionChanged(object sender, System.EventArgs e)
      {
         this.NoComics = this.Files.Count == 0;
         Engine.Statistics.Execute();
      }

      #endregion


      #region NoComics
      bool _NoComics;
      public bool NoComics
      {
         get { return this._NoComics; }
         set { this.SetProperty(ref this._NoComics, value); }
      }
      #endregion

      #region ClearAll
      internal void ClearAll()
      {
         try
         {
            if (this.Folders.Count != 0)
            { this.Folders.ReplaceRange(new List<Folder.FolderData>()); }

            if (this.Files.Count != 0)
            { this.Files.ReplaceRange(new List<File.FileData>()); }

            if (this.ReadingFiles.Count != 0) { this.ReadingFiles.Clear(); }
            if (this.RecentFiles.Count != 0) { this.RecentFiles.Clear(); }
            if (this.TopRatedFiles.Count != 0) { this.TopRatedFiles.Clear(); }

         }
         catch { }
      }
      #endregion

   }
}