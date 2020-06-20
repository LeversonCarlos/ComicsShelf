using ComicsShelf.Observables;
using ComicsShelf.ViewModels;
using System;

namespace ComicsShelf.Settings
{
   public class LibrarySetting : ObservableObject, IDisposable
   {

      public LibraryVM Library { get; }

      public LibrarySetting(LibraryVM library)
      {
         this.Library = library;
      }

      public void Dispose()
      {
         throw new NotImplementedException();
      }

   }
}
