using System.Linq;

namespace ComicsShelf.ViewModels
{
   public class FolderVM
   {

      public FolderVM(string path)
      {
         this.Path = path;
         this.Text = System.IO.Path.GetFileNameWithoutExtension(path);
      }

      public string Text { get; set; }
      public string Path { get; set; }

      public FolderVM[] Folders { get; set; }

      ItemVM[] _Items;
      public ItemVM[] Items
      {
         get => _Items;
         set
         {
            _Items = value;
            FirstItem = value?.FirstOrDefault();
         }
      }
      public ItemVM FirstItem { get; set; }

   }
}
