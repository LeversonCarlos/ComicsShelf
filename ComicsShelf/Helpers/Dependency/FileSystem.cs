using ComicsShelf.ComicFiles;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers
{

   public class Folder
   {
      public string Path { get; set; }
      public string Name { get; set; }
   }

   public class File
   {
      public string FileKey { get; set; }
      public string FilePath { get; set; }
      public string FolderPath { get; set; }
      public string Text { get; set; }
   }

   public interface IFileSystem : IDisposable
   {

      Task<bool> Validate(string libraryKey);
      string PathSeparator { get; }

      Task<Folder> GetRootPath();
      Task<Folder[]> GetFolders(Folder folder);
      Task<File[]> GetFiles(Folder folder);

      Task<byte[]> LoadData(Libraries.LibraryModel library);
      Task<bool> SaveData(Libraries.LibraryModel library, byte[] serializedData);

      Task ExtractCover(Libraries.LibraryModel library, ComicFile comicFile);
      Task<ComicPagesVM> ExtractPages(Libraries.LibraryModel library, ComicFile comicFile);

   }
}