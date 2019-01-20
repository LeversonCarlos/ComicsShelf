﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ComicsShelf.UWP
{
   partial class FileSystem
   {

      public async Task CoverExtract(Helpers.Database.dbContext database, Helpers.Database.ComicFile comicFile)
      {
         try
         {

            // OPEN ZIP ARCHIVE
            var comicStorageFile = await GetStorageFile(comicFile.LibraryPath, comicFile.FullPath);
            if (comicStorageFile == null) { return; }
            using (var zipArchiveStream = await comicStorageFile.OpenStreamForReadAsync())
            {
               using (var zipArchive = new ZipArchive(zipArchiveStream, ZipArchiveMode.Read))
               {

                  // LOCATE FIRST JPG ENTRY
                  var zipEntry = zipArchive.Entries
                     .Where(x => x.Name.ToLower().EndsWith(".jpg"))
                     .OrderBy(x => x.Name)
                     .Take(1)
                     .FirstOrDefault();

                  // OPEN STREAM
                  using (var zipEntryStream = zipEntry.Open())
                  {
                     await this.SaveThumbnail(zipEntryStream, comicFile.CoverPath);
                     zipEntryStream.Close();
                     zipEntryStream.Dispose();
                  }

                  // RELEASE DATE
                  System.IO.File.SetLastWriteTime(comicFile.CoverPath, zipEntry.LastWriteTime.DateTime.ToLocalTime());
                  comicFile.ReleaseDate = database.GetDate(zipEntry.LastWriteTime.DateTime.ToLocalTime());
                  await Task.Run(() => database.Update(comicFile));

                  zipArchive.Dispose();
               }

               zipArchiveStream.Close();
               zipArchiveStream.Dispose();
            }

         }
         catch { }
      }

      public async Task SaveThumbnail(Stream imageStream, string imagePath)
      {
         try
         {
            using (var thumbnailFile = new FileStream(imagePath, FileMode.CreateNew, FileAccess.Write))
            {
               await imageStream.CopyToAsync(thumbnailFile);
               await thumbnailFile.FlushAsync();
               thumbnailFile.Close();
               thumbnailFile.Dispose();
            }
         }
         catch (Exception) { throw; }
      }

   }
}