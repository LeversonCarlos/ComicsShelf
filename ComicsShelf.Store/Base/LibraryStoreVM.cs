using System.Collections.Generic;

namespace ComicsShelf.Store
{
   public enum enLibraryType : short { LocalDrive = 0, OneDrive = 1 }
   public class LibraryStoreVM
   {
      public string ID { get; set; }
      public string Description { get; set; }
      public enLibraryType Type { get; set; }
      public Dictionary<string, string> KeyValues { get; set; }
   }
}
