using System;
using System.Threading;

namespace ComicsShelf.Library
{
   internal class Service
   {
      static Lazy<IService> FileSystem = new Lazy<IService>(() => new Implementation.FileSystemService(), LazyThreadSafetyMode.PublicationOnly);
      static Lazy<IService> OneDrive = new Lazy<IService>(() => new Implementation.OneDrive(), LazyThreadSafetyMode.PublicationOnly);
      public static IService Get(Helpers.Database.Library library)
      {
         IService ret = null;
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