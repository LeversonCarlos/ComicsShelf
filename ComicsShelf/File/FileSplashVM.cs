using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.File
{
   public class FileSplashVM : Helpers.ViewModels.DataVM<FileData>
   {

      #region New
      public FileSplashVM(FileData args)
      {
         this.Title = args.Text;
         this.ViewType = typeof(FileSplashPage);
         this.Data = args;
         this.OpenTappedCommand = new Command(async (item) => await this.OpenTapped(item));
      }
      #endregion

      #region OpenTapped
      public Command OpenTappedCommand { get; set; }
      private async Task OpenTapped(object item)
      {
         try
         {
            await PushAsync<FileReadVM>(this.Data);
         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

   }
}