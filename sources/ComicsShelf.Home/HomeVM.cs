using ComicsShelf.Observables;
using ComicsShelf.ViewModels;

namespace ComicsShelf.Home
{
   public class HomeVM : BaseVM
   {

      public ObservableList<FolderVM> Sections { get; }

      public HomeVM()
      {
         this.Title = "comics shelf tester";
         this.Sections = new ObservableList<FolderVM>();
         Notifyers.Notify.SectionsUpdate(sections => this.Sections.ReplaceRange(sections));
      }

   }
}
