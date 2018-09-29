using System.Collections.Generic;

namespace ComicsShelf.Views.Home
{
   public class HomeData : Folder.FolderData
   {

      #region New
      public HomeData() : base(new Helpers.Database.ComicFolder { FullText = R.Strings.AppTitle })
      {
         this.RecentFiles = new Helpers.Observables.ObservableList<File.FileData>();
         this.RecentFiles.CollectionChanged += this.RecentFiles_CollectionChanged;

         this.ReadingFiles = new Helpers.Observables.ObservableList<File.FileData>();
         this.ReadingFiles.CollectionChanged += this.ReadingFiles_CollectionChanged;

         this.TopRatedFiles = new Helpers.Observables.ObservableList<File.FileData>();
         this.TopRatedFiles.CollectionChanged += this.TopRatedFiles_CollectionChanged;

         this.Files.ObservableCollectionChanged += this.Files_CollectionChanged;
      }
      #endregion


      #region RecentFiles

      public Helpers.Observables.ObservableList<File.FileData> RecentFiles { get; set; }

      bool _HasRecentFiles;
      public bool HasRecentFiles
      {
         get { return this._HasRecentFiles; }
         set { this.SetProperty(ref this._HasRecentFiles, value); }
      }

      private void RecentFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      { this.HasRecentFiles = this.RecentFiles.Count != 0; }

      #endregion

      #region ReadingFiles

      public Helpers.Observables.ObservableList<File.FileData> ReadingFiles { get; set; }

      bool _HasReadingFiles;
      public bool HasReadingFiles
      {
         get { return this._HasReadingFiles; }
         set { this.SetProperty(ref this._HasReadingFiles, value); }
      }

      private void ReadingFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      { this.HasReadingFiles = this.ReadingFiles.Count != 0; }

      #endregion

      #region TopRatedFiles

      public Helpers.Observables.ObservableList<File.FileData> TopRatedFiles { get; set; }

      bool _HasTopRatedFiles;
      public bool HasTopRatedFiles
      {
         get { return this._HasTopRatedFiles; }
         set { this.SetProperty(ref this._HasTopRatedFiles, value); }
      }

      private void TopRatedFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      { this.HasTopRatedFiles = this.TopRatedFiles.Count != 0; }

      #endregion

      #region CollectionChanged

      private void Files_CollectionChanged(object sender, System.EventArgs e)
      { Engine.Statistics.Execute(); }

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

            if (this.ReadingFiles.Count != 0)
            { this.ReadingFiles.Clear(); this.HasReadingFiles = false; }

            if (this.RecentFiles.Count != 0)
            { this.RecentFiles.Clear(); this.HasRecentFiles = false; }

            if (this.TopRatedFiles.Count != 0)
            { this.TopRatedFiles.Clear(); this.HasTopRatedFiles = false; }

         }
         catch { }
      }
      #endregion

   }
}