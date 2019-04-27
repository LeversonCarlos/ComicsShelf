using ComicsShelf.Helpers.Observables;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.FolderDialog
{
   public class FolderDialogVM : BaseVM
   {

      readonly ObservableList<FolderData> Data;
      readonly TaskCompletionSource<FolderData> tcs;
      public FolderDialogVM()
      {
         this.Title = R.Strings.FOLDER_DIALOG_TITLE;
         this.Data = new ObservableList<FolderData>();
         this.ConfirmCommand = new Command(async () => await this.Confirm());
         this.CancelCommand = new Command(async () => await this.Cancel());
         this.ItemSelectCommand = new Command((item) => this.ItemSelect(item as FolderData));
         this.tcs = new TaskCompletionSource<FolderData>();
      }

      FolderData _CurrentItem;
      public FolderData CurrentItem
      {
         get { return this._CurrentItem; }
         set { this.SetProperty(ref this._CurrentItem, value); }
      }

      public EventHandler<FolderData> OnItemSelected;
      public Command ItemSelectCommand { get; set; }
      void ItemSelect(FolderData item)
      {
         this.OnItemSelected?.Invoke(this, item);
      }

      public Command ConfirmCommand { get; set; }
      async Task Confirm()
      {
         await this.ClosePage();
         tcs.SetResult(this.CurrentItem);
      }

      public Command CancelCommand { get; set; }
      async Task Cancel()
      {
         await this.ClosePage();
         tcs.SetResult(null);
      }

      public async Task<FolderData> OpenPage()
      {
         var mainPage = Application.Current.MainPage as Page;
         var view = new FolderDialogPage();
         view.BindingContext = this;
         await mainPage.Navigation.PushModalAsync(view, true);
         return await this.tcs.Task;
      }

      async Task ClosePage()
      {
         var mainPage = Application.Current.MainPage as Page;
         await mainPage.Navigation.PopModalAsync(true);
      }

   }
}
