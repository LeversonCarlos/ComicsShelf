using ComicsShelf.FolderSelector;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.FolderDialog
{
   internal class FolderDialogVM : BaseVM
   {
      readonly TaskCompletionSource<SelectorItemVM> tcs;
      public ObservableList<SelectorItemVM> Data { get; private set; }

      public FolderDialogVM()
      {
         this.Title = Resources.Translations.DRIVE_FOLDER_DIALOG_TITLE;
         this.Data = new ObservableList<SelectorItemVM>();
         this.tcs = new TaskCompletionSource<SelectorItemVM>();
         this.ItemSelectCommand = new Command(async item => await this.ItemSelect(item));
         this.ConfirmCommand = new Command(async () => await this.Confirm());
         this.CancelCommand = new Command(async () => await this.Cancel());
      }

      SelectorItemVM _CurrentItem;
      public SelectorItemVM CurrentItem
      {
         get { return this._CurrentItem; }
         set { this.SetProperty(ref this._CurrentItem, value); }
      }

      public EventHandler<SelectorItemVM> OnItemSelected;
      public Command ItemSelectCommand { get; set; }
      internal async Task ItemSelect(object item)
      {
         this.OnItemSelected?.Invoke(this, item as SelectorItemVM);
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

      public async Task<SelectorItemVM> OpenPage()
      {
         var view = new FolderDialog.Page { BindingContext = this };
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
