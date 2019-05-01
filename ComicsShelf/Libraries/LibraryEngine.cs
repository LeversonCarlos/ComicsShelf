using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Libraries
{
   internal class LibraryEngine
   {

      public async static Task<ShellItem> Add(LibraryModel library)
      {
         try
         {

            var libraryVM = new LibraryVM(library);
            var shellContent = new ShellContent
            {
               Title = library.Description,
               BindingContext = libraryVM,
               ContentTemplate = new DataTemplate(typeof(LibraryPage))
            };

            var shellSection = new ShellSection { Title = library.Description };
            shellSection.Items.Add(shellContent);

            var shellItem = new ShellItem { Title = library.Description, Icon = $"icon_{library.Type.ToString()}.png" };
            shellItem.Items.Add(shellSection);

            Shell.CurrentShell.Items.Add(shellItem);
            await libraryVM.LoadFiles();
            return shellItem;

         }
         catch (Exception) { throw; }
      }

      public static async void LoadFiles(LibraryModel library)
      {
         var engine = Engines.Engine.Get(library.Type);
         if (engine == null) { return; }
         var files = await engine.SearchFiles(new Helpers.Folder { Path = library.LibraryKey });
         Messaging.Send("LoadFiles", library.LibraryKey, files);
      }

   }
}
