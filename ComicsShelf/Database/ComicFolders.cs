using SQLite;

namespace ComicsShelf.Database
{
   [Table("ComicFolders")]
   public class ComicFolders 
   {

      [PrimaryKey]
      public string FullPath { get; set; }

      public string Text { get; set; }

      public string ParentPath { get; set; }
      public string CoverPath { get; set; }

   }
}