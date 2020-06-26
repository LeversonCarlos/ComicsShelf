using System.Collections.Generic;

namespace ComicsShelf.ViewModels
{
   public class SectionVM
   {

      public SectionVM(string text, LibraryVM library)
      {
         Library = library;
         Text = text;
         Folders = new List<FolderVM>();
      }

      public LibraryVM Library { get; private set; }
      public string Text { get; private set; }

      public IList<FolderVM> Folders { get; private set; }

   }
}
