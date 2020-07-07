using System;
using System.Collections.Generic;

namespace ComicsShelf.FolderSelector
{
   internal enum enSelectorItemType : short { Drive, Folder, File }

   internal class SelectorItemVM
   {

      public string ID { get; set; }
      public string Name { get; set; }
      public string Path { get; set; }

      public string ParentID { get; set; }

      public DateTime? CreatedDateTime { get; set; }
      public long? SizeInBytes { get; set; }
      public Dictionary<string, string> KeyValues { get; set; }

      internal SelectorItemVM Parent { get; set; }
      internal enSelectorItemType Type { get; set; }
   }

}
