using SQLite;
using System;

namespace ComicsShelf.Helpers.Settings
{
   [Table("Comics")]
   public class Comics
   {

      [PrimaryKey]
      public string FullPath { get; set; }

      public string ReleaseDate { get; set; }

      public bool Readed { get; set; }
      public string ReadingDate { get; set; }

      public short? Rate { get; set; }

   }
}