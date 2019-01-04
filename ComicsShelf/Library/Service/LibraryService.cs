using System;
using System.Threading;

namespace ComicsShelf.Library
{
   internal class LibraryService
   {
      static Lazy<ILibraryService> FileSystem = new Lazy<ILibraryService>(() => new Implementation.FileSystemService(), LazyThreadSafetyMode.PublicationOnly);
      static Lazy<ILibraryService> OneDrive = new Lazy<ILibraryService>(() => new Implementation.OneDrive(), LazyThreadSafetyMode.PublicationOnly);
      public static ILibraryService Get(Helpers.Database.Library library)
      {
         ILibraryService ret = null;
         if (library.Type == LibraryTypeEnum.FileSystem)
         { ret = FileSystem.Value; }
         if (library.Type == LibraryTypeEnum.OneDrive)
         { ret = OneDrive.Value; }
         if (ret == null)
         { throw new NotImplementedException($"This functionality is not implemented for {library.Type.ToString()} library type."); }
         return ret;
      }

   }
}