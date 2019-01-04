namespace ComicsShelf.Library.Implementation
{
   internal partial class FileSystemService : ILibraryService
   {

      Helpers.iFileSystem FileSystem { get; set; }
      public FileSystemService()
      {
         this.FileSystem = Helpers.FileSystem.Get();
      }

   }
}