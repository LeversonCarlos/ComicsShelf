using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Home
{
   public class HomeVM : Helpers.ViewModels.DataVM<HomeData>
   {

      #region New
      public HomeVM(HomeData args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(HomePage);

         this.Data = args;
         this.FolderTappedCommand = new Command(async (item) => await this.FolderTapped(item));
         this.FileTappedCommand = new Command(async (item) => await this.FileTapped(item));
      }
      #endregion

      #region FolderTapped
      public Command FolderTappedCommand { get; set; }
      private async Task FolderTapped(object item)
      {
         try
         {
            var folderItem = (Folder.FolderData)item;
            await PushAsync<Folder.FolderVM>(folderItem);
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

      #region FileTapped
      public Command FileTappedCommand { get; set; }
      private async Task FileTapped(object item)
      {
         try
         {
            var fileItem = (File.FileData)item;
            await PushAsync<File.FileSplashVM>(fileItem);
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

   }
}