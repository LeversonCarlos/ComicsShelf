using System;
using System.Collections.Generic;

namespace ComicsShelf.Store
{
   public class ItemStoreVM
   {
      public string ID { get; set; }
      public string LibraryID { get; set; }

      public string FullDescription { get; set; }
      public string ShortDescription { get; set; }

      public string FolderPath { get; set; }
      public string FileName { get; set; }

      public short? Rating { get; set; }
      public bool Readed { get; set; }
      public DateTime? ReadingDate { get; set; }
      public short? ReadingPage { get; set; }
      public double ReadingPercent { get; set; }

      public Dictionary<string, string> KeyValues { get; set; }
      public bool Available { get; set; }
   }
}
