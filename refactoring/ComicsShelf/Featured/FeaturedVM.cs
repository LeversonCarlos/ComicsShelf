using ComicsShelf.Helpers.Observables;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComicsShelf.Featured
{
   public class FeaturedVM : BaseVM
   {

      public readonly Store.LibraryModel Library;
      public FeaturedVM(Store.LibraryModel Library)
      {
         this.Library = Library;
      }

   }
}
