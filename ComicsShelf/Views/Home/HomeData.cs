using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Internals;

namespace ComicsShelf.Views.Home
{
   public class HomeData : Folder.FolderData
   {

      #region New
      public HomeData() : base(new Helpers.Database.ComicFolder { Text = R.Strings.AppTitle })
      {
         this.NoComics = true;

         this.Libraries = new Helpers.Observables.ObservableList<LibraryData>();
         this.Libraries.Add(new EmptyData());
         this.Libraries.ObservableCollectionChanged += this.Libraries_CollectionChanged;
         this.Files.ObservableCollectionChanged += this.Files_CollectionChanged;
      }
      #endregion


      #region EmptyCoverImage
      Xamarin.Forms.ImageSource _EmptyCoverImage = null;
      public Xamarin.Forms.ImageSource EmptyCoverImage
      {
         get { return this._EmptyCoverImage; }
         set { this.SetProperty(ref this._EmptyCoverImage, value); }
      }
      #endregion

      #region Featured

      private LibraryData FeaturedLibrary()
      { return this.Libraries.Where(x => x.IsFeaturedPage).FirstOrDefault(); } 

      private Folder.FolderData FeaturedSection(string featuredSectionName)
      {
         var featuredLibrary = this.FeaturedLibrary();
         if (featuredLibrary == null) { return null; }
         return featuredLibrary.Folders.Where(x => x.ComicFolder.Key == featuredSectionName).FirstOrDefault();
      }

      private Helpers.Observables.ObservableList<File.FileData> FeaturedSectionFiles(string featuredSectionName)
      {
         var featuredSection = this.FeaturedSection(featuredSectionName);
         if (featuredSection == null) { return new Helpers.Observables.ObservableList<File.FileData>(); }
         return featuredSection.Files;
      }

      public Helpers.Observables.ObservableList<File.FileData> RecentFiles
      { get { return this.FeaturedSectionFiles("RECENT_FILES"); } }

      public Helpers.Observables.ObservableList<File.FileData> ReadingFiles
      { get { return this.FeaturedSectionFiles("READING_FILES"); } }

      public Helpers.Observables.ObservableList<File.FileData> TopRatedFiles
      { get { return this.FeaturedSectionFiles("TOP_RATED_FILES"); } }

      #endregion

      #region Libraries

      public Helpers.Observables.ObservableList<LibraryData> Libraries { get; set; }

      private void Libraries_CollectionChanged(object sender, System.EventArgs e)
      {
         if (this.Libraries.Count == 0)
         { this.Libraries.Add(new EmptyData()); }
         else {
            if (this.Libraries.Count(x => x.IsEmptyPage == false) > 0) {
               var empty = this.Libraries.Where(x => x.IsEmptyPage == true).FirstOrDefault();
               if (empty != null)
               { this.Libraries.Remove(empty); }
            }
         }
      }

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

   }
}