using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComicsShelf.Views.Home
{
   public class FeaturedData : Views.Home.LibraryData
   {

      public FeaturedData() : base(new Helpers.Database.ComicFolder { Key = "FEATURED", Text = R.Strings.HOME_FEATURED_PAGE_TITLE })
      {

         var readingFiles = new Helpers.Database.ComicFolder { Key = "READING_FILES", Text = R.Strings.HOME_READING_FILES_SECTION_TITLE };
         var readingFilesData = new Views.Folder.FolderData(readingFiles);
         readingFilesData.Files.CollectionChanged += this.Files_CollectionChanged;
         this.Folders.Add(readingFilesData);

         var recentFiles = new Helpers.Database.ComicFolder { Key = "RECENT_FILES", Text = R.Strings.HOME_RECENT_FILES_SECTION_TITLE };
         var recentFilesData = new Views.Folder.FolderData(recentFiles);
         recentFilesData.Files.CollectionChanged += this.Files_CollectionChanged;
         this.Folders.Add(recentFilesData);

         var topRatedFiles = new Helpers.Database.ComicFolder { Key = "TOP_RATED_FILES", Text = R.Strings.HOME_TOP_RATED_SECTION_TITLE };
         var topRatedFilesData = new Views.Folder.FolderData(topRatedFiles);
         topRatedFilesData.Files.CollectionChanged += this.Files_CollectionChanged;
         this.Folders.Add(topRatedFilesData);

         this.HasFolders = true;
         this.IsFeaturedPage = true;
      }

      private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
         {
            var hasFiles = false;
            foreach (var folder in this.Folders)
            {
               folder.HasFiles = folder.Files.Count != 0;
               if (folder.HasFiles) { hasFiles = true; }
            }
            this.HasFiles = hasFiles;
            this.IsEmptyLibrary = !this.HasFiles;
         });
      }

   }
}
