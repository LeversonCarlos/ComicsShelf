using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Folder
{
   public class FolderVM : Helpers.ViewModels.DataVM<FolderData>
   {

      #region New
      public FolderVM(FolderData args)
      {
         this.Title = R.Strings.AppTitle + ": " + args.Text;
         this.ViewType = typeof(FolderPage);

         this.Data = args;
         this.HasFolders = this.Data.Folders.Count != 0;
         // this.HasFiles = this.Data.Files.Count != 0;
         this.FolderTappedCommand = new Command(async (item) => await this.FolderTapped(item));
         // this.FileTappedCommand = new Command(async (item) => await this.FileTapped(item));
      }
      #endregion

      #region Properties
      public bool HasFolders { get; set; }
      #endregion

      #region FolderTapped
      public Command FolderTappedCommand { get; set; }
      private async Task FolderTapped(object item)
      {
         try
         {
            var folderItem = (FolderData)item;
            await PushAsync<FolderVM>(folderItem);
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

   }
}