using System.Collections.Generic;

namespace ComicsShelf.ViewModels
{

   public enum enLibraryType : short { LocalDrive = 0, OneDrive = 1 }

   public class LibraryVM
   {
      public LibraryVM() { KeyValues = new Dictionary<string, string>(); }
      public string ID { get; set; }
      public string EscapedID { get; set; }
      public string Description { get; set; }
      public string Path { get; set; }
      public enLibraryType Type { get; set; }
      public Dictionary<string, string> KeyValues { get; set; }
   }

}
