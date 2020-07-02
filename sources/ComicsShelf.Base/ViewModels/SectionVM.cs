using System.Collections.Generic;

namespace ComicsShelf.ViewModels
{
   public class SectionVM
   {

      public SectionVM(string text, LibraryVM library)
      {
         Library = library;
         Text = text;
         SizeFactor = 1;
         Folders = new List<FolderVM>();
      }

      public LibraryVM Library { get; private set; }
      public string Text { get; private set; }

      public double SizeFactor { get; set; }
      public double SizeHeight => Helpers.Cover.DefaultHeight * SizeFactor;
      public double SizeWidth => Helpers.Cover.DefaultWidth * SizeFactor;

      public IList<FolderVM> Folders { get; private set; }

   }
}
