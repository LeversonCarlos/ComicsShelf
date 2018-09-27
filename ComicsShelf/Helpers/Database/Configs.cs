using SQLite;

namespace ComicsShelf.Helpers.Database
{
   public class Configs
   {

      [PrimaryKey, AutoIncrement]
      public int ConfigID { get; set; }

      public string LibraryPath { get; set; }

   }
}