using ComicsShelf.Helpers.Observables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.OneDrive.Files;

namespace ComicsShelf.Libraries.Service.OneDrive.FolderSelector
{
   public class SelectorVM : Helpers.DataVM<ObservableList<FileData>>
   {
      TaskCompletionSource<FileData> tcs;

      #region New
      public SelectorVM()
      {
         this.Title = "[FolderSelector]";
         this.ViewType = typeof(SelectorPage);
         this.Data = new ObservableList<FileData>();
         this.ConfirmCommand = new Command(async () => await this.Confirm());
         this.CancelCommand = new Command(async () => await this.Cancel());
         this.ItemSelectCommand = new Command((item) => this.ItemSelect(item as FileData));
         this.tcs = new TaskCompletionSource<FileData>();
      }
      #endregion


      #region CurrentItem
      FileData _CurrentItem;
      public FileData CurrentItem
      {
         get { return this._CurrentItem; }
         set { this.SetProperty(ref this._CurrentItem, value); }
      }
      #endregion


      #region ItemSelectCommand
      public EventHandler<FileData> OnItemSelected;
      public Command ItemSelectCommand { get; set; }
      void ItemSelect(FileData item)
      {
         this.OnItemSelected?.Invoke(this, item);
      }
      #endregion

      #region ConfirmCommand
      public Command ConfirmCommand { get; set; }
      async Task Confirm()
      {
         tcs.SetResult(this.CurrentItem);
         await this.ClosePage();
      }
      #endregion

      #region CancelCommand
      public Command CancelCommand { get; set; }
      async Task Cancel()
      {
         tcs.SetResult(null);
         await this.ClosePage();
      }
      #endregion

      #region OpenAndClose

      Page mainPage;

      public async Task<FileData> OpenPage()
      {
         this.mainPage = Application.Current.MainPage as Page;
         var view = new SelectorPage();
         view.BindingContext = this;
         await this.mainPage.Navigation.PushModalAsync(view, true);
         return await this.tcs.Task;
      }

      async Task ClosePage()
      {
         await this.mainPage.Navigation.PopModalAsync(true);
      }

      #endregion


   }
}