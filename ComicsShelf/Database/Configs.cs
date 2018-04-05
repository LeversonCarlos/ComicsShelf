using SQLite;

namespace ComicsShelf.Database
{
   public class Configs
    {

      [PrimaryKey, AutoIncrement]
      public int ConfigID { get; set; }

      public string ComicsPath { get; set; }

   }
}
