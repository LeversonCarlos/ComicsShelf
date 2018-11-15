using SQLite;

namespace ComicsShelf.Helpers.Database
{
   [Table("Libraries")]
   public class Library
   {

      [PrimaryKey, AutoIncrement]
      public int LibraryID { get; set; }

      [Indexed]
      public string LibraryPath { get; set; }

      public string LibraryText { get; set; }
      public bool Available { get; set; }
      public int FileCount { get; set; }

   }
}