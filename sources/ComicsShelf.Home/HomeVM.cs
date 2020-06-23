using ComicsShelf.Observables;
using ComicsShelf.ViewModels;

namespace ComicsShelf.Home
{
   public class HomeVM : BaseVM
   {

      public ObservableList<FolderVM> Sections { get; }

      public HomeVM()
      {
         Title = Strings.TITLE;
         Sections = new ObservableList<FolderVM>();
         Notifyers.Notify.SectionsUpdate(sections => Sections.ReplaceRange(sections));
      }

   }
}
