using ComicsShelf.Helpers.Observables;

namespace ComicsShelf.ComicFiles
{
   public class ComicPagesVM : ObservableObject
   {

      public ObservableList<ComicPageVM> Pages { get; internal set; }
      public ComicPagesVM()
      {
         this.Pages = new ObservableList<ComicPageVM>();
      }

   }
}
