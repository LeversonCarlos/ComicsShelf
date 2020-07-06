using ComicsShelf.ViewModels;
using System;
using Xamarin.CloudDrive.Connector;
using Xamarin.Forms;

namespace ComicsShelf.Drive
{

   public abstract partial class BaseDrive<T> : BaseDrive, IDrive where T : ICloudDriveService
   {

      public virtual enLibraryType LibraryType => throw new NotImplementedException();

      protected string[] ImageExtentions => new string[] { ".jpg", ".jpeg", ".png" };

      public ICloudDriveService CloudService { get { return ImplementationProvider.Get<T>(); } }

      public virtual string EscapeFileID(string fileID) => throw new NotImplementedException();

      protected Interfaces.IFileSystem FileSystem => DependencyService.Get<Interfaces.IFileSystem>();

   }

   public class BaseDrive
   {

      public static IDrive GetDrive(enLibraryType libraryType)
      {
         try
         {
            if (libraryType == enLibraryType.OneDrive)
               return DependencyService.Get<OneDrive>();
            else if (libraryType == enLibraryType.LocalDrive)
               return DependencyService.Get<LocalDrive>();
            else
               throw new NotImplementedException($"Drive for library type {libraryType} not implemented");
         }
         catch (Exception) { throw; }
      }

   }

}
