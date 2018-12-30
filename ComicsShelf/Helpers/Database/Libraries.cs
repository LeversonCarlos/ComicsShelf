using SQLite;

namespace ComicsShelf.Helpers.Database
{
   public enum LibraryTypeEnum : short { FileSystem = 0, OneDrive = 1 }

   [Table("Libraries")]
   public class Library
   {

      [PrimaryKey, AutoIncrement]
      public int LibraryID { get; set; }

      public string LibraryText { get; set; }

      [Indexed]
      public string LibraryPath { get; set; }

      [Ignore]
      public LibraryTypeEnum Type
      {
         get { return (LibraryTypeEnum)this.TypeInner; }
         set { this.TypeInner = (short)value; }
      }
      public short TypeInner { get; set; }

      public bool Available { get; set; }
      public int FileCount { get; set; }

   }
}