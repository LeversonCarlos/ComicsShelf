using ComicsShelf.Helpers.Observables;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.FolderDialog
{
   internal class FolderDialogVM : BaseVM
   {

      private readonly TaskCompletionSource<Folder> tcs;
      public FolderDialogVM()
      {
         this.Title = R.Strings.FOLDER_DIALOG_TITLE;
         this.Data = new ObservableList<Folder>();
         this.ConfirmCommand = new Command(async () => await this.Confirm());
         this.CancelCommand = new Command(async () => await this.Cancel());
         this.ItemSelectCommand = new Command((item) => this.ItemSelect(item));
         this.tcs = new TaskCompletionSource<Folder>();
      }

      public ObservableList<Folder> Data { get; private set; }
      public Folder SelectedItem { get; set; }

      Folder _CurrentItem;
      public Folder CurrentItem
      {
         get { return this._CurrentItem; }
         set { this.SetProperty(ref this._CurrentItem, value); }
      }

      public EventHandler<Folder> OnItemSelected;
      public Command ItemSelectCommand { get; set; }
      void ItemSelect(object item)
      {
         this.OnItemSelected?.Invoke(this, this.SelectedItem);
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

      public async Task<Folder> OpenPage()
      {
         var mainPage = Application.Current.MainPage as Page;
         var view = new FolderDialogPage { BindingContext = this };
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
