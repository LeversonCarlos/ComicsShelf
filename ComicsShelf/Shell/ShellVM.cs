using ComicsShelf.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf
{
   public class ShellVM : BaseVM
   {

      public ShellVM()
      {
         this.NewLibraryCommand = new Command(() => this.NewLibrary());

      }

      public Command NewLibraryCommand { get; set; }
      private void NewLibrary()
      {
         this.IsBusy = true;
         try
         {

            var libraryStore = DependencyService.Get<Libraries.LibraryStore>();
            var library = new Libraries.LibraryModel
            {
               Key = DateTime.Now.Ticks.ToString(),
               Description = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };
            libraryStore.AddLibrary(library);

            this.AddLibrary(library);

         }
         catch (Exception) { throw; }
         this.IsBusy = false;
      }

      private void AddLibrary(Libraries.LibraryModel library)
      {
         try
         {

            var shellContent = new ShellContent
            {
               Title = library.Description,
               BindingContext = new Libraries.LibraryVM(library),
               ContentTemplate = new DataTemplate(typeof(Libraries.LibraryPage))
            };

            var shellSection = new ShellSection { Title = library.Description };
            shellSection.Items.Add(shellContent);

            var shellItem = new ShellItem { Title = library.Description };
            shellItem.Items.Add(shellSection);

            Shell.CurrentShell.Items.Add(shellItem);

         }
         catch (Exception) { throw; }
      }

   }
}
