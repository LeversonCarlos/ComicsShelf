using System;
using System.Threading;

namespace ComicsShelf.Libraries
{
   internal class LibraryService
   {
      static Lazy<ILibraryService> FileSystem = new Lazy<ILibraryService>(() => new Implementation.FileSystemService(), LazyThreadSafetyMode.PublicationOnly);
      static Lazy<ILibraryService> OneDrive = new Lazy<ILibraryService>(() => new Implementation.OneDrive(), LazyThreadSafetyMode.PublicationOnly);
      public static ILibraryService Get(Library library)
      {
         ILibraryService ret = null;
         if (library.Type == TypeEnum.FileSystem)
         { ret = FileSystem.Value; }
         if (library.Type == TypeEnum.OneDrive)
         { ret = OneDrive.Value; }
         if (ret == null)
         { throw new NotImplementedException($"This functionality is not implemented for {library.Type.ToString()} library type."); }
         return ret;
      }

   }
}