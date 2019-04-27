using SQLite;

namespace ComicsShelf.Helpers.Database
{
   // [Table("ComicFolders")]
   public class ComicFolder
   {

      // [PrimaryKey]
      public string Key { get; set; }

      // [Indexed]
      public string LibraryPath { get; set; }
      // [Indexed]
      public string FullPath { get; set; }
      public string ParentPath { get; set; }
      public string CoverPath { get; set; }

      public string Text { get; set; }

   }
}