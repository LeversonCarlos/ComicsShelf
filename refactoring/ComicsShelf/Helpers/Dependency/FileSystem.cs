using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers
{

   public class Folder
   {
      public string Key { get; set; }
      public string FullPath { get; set; }
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

      string PathSeparator { get; }
      Task<bool> Validate(string libraryKey);
      Task<Folder> GetRootPath();
      Task<Folder[]> GetFolders(Folder folder);

      //Task<File[]> GetFiles(Folder folder);

      //Task<byte[]> LoadData(LibraryModel library);
      //Task<bool> SaveData(LibraryModel library, byte[] serializedData);

      //Task<System.Drawing.Size> GetPageSize(string pagePath);
      //Task SaveThumbnail(System.IO.Stream imageStream, string imagePath);

      //Task ExtractCover(LibraryModel library, ComicFile comicFile);
      //Task<List<ComicPageVM>> ExtractPages(LibraryModel library, ComicFile comicFile);

   }
}