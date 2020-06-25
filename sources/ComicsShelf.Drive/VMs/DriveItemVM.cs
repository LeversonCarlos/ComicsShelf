using System;
using System.Collections.Generic;

namespace ComicsShelf.Drive
{
   internal enum enItemType : short { Drive, Folder, File }

   internal class DriveItemVM
   {

      public string ID { get; set; }
      public string Name { get; set; }
      public string Path { get; set; }

      public string ParentID { get; set; }

      public DateTime? CreatedDateTime { get; set; }
      public long? SizeInBytes { get; set; }
      public Dictionary<string, string> KeyValues { get; set; }

      internal DriveItemVM Parent { get; set; }
      internal enItemType Type { get; set; }
   }

}
