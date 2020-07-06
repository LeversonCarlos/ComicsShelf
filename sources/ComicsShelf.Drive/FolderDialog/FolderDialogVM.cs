using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using System.Transactions;
using Xamarin.Forms;

namespace ComicsShelf.Drive.FolderDialog
{
   internal class FolderDialogVM : BaseVM
   {
      readonly TaskCompletionSource<DriveItemVM> tcs;
      public ObservableList<DriveItemVM> Data { get; private set; }

      public string SELECTED_PATH => Translations.FOLDER_DIALOG_SELECTED_PATH;
      public string CANCEL_COMMAND => Translations.FOLDER_DIALOG_CANCEL_COMMAND;
      public string CONFIRM_COMMAND => Translations.FOLDER_DIALOG_CONFIRM_COMMAND;

      public FolderDialogVM()
      {
         this.Title = Translations.FOLDER_DIALOG_TITLE;
         this.Data = new ObservableList<DriveItemVM>();
         this.tcs = new TaskCompletionSource<DriveItemVM>();
         this.ItemSelectCommand = new Command(async item => await this.ItemSelect(item));
         this.ConfirmCommand = new Command(async () => await this.Confirm());
         this.CancelCommand = new Command(async () => await this.Cancel());
      }

      DriveItemVM _CurrentItem;
      public DriveItemVM CurrentItem
      {
         get { return this._CurrentItem; }
         set { this.SetProperty(ref this._CurrentItem, value); }
      }

      public EventHandler<DriveItemVM> OnItemSelected;
      public Command ItemSelectCommand { get; set; }
      internal async Task ItemSelect(object item)
      {
         this.OnItemSelected?.Invoke(this, item as DriveItemVM);
         await Task.CompletedTask;
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

      public async Task<DriveItemVM> OpenPage()
      {
         var view = new FolderDialogPage { BindingContext = this };
         await Helpers.Modal.Push(view);
         return await this.tcs.Task;
      }

      async Task ClosePage()
      {
         this.IsBusy = true;
         await Helpers.Modal.Pop();
         this.IsBusy = false;
      }

   }
}
