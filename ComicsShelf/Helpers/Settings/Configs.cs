using SQLite;

namespace ComicsShelf.Helpers.Settings
{
   public class Configs
    {

      [PrimaryKey, AutoIncrement]
      public int ConfigID { get; set; }

      public string ComicsPath { get; set; }

   }
}
