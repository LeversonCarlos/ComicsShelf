using System;

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
         this.Initialize += this.OnInitialize;
         // this.Finalize += this.OnFinalize;
         // this.FolderTappedCommand = new Command(async (item) => await this.FolderTapped(item));
         // this.FileTappedCommand = new Command(async (item) => await this.FileTapped(item));
      }
      #endregion

      #region Properties
      public bool HasFolders { get; set; }
      #endregion

      #region OnInitialize
      private async void OnInitialize()
      {
         try
         {
            if (this.IsBusy) { return; }
            this.IsBusy = true;

            /*
            foreach (var folderData in this.Data.Folders)
            { this.OnInitialize_Folders(folderData); }
            this.OnInitialize_Folders(this.Data);
            // await this.OnInitialize_Files();
            */

            if (await App.Message.Confirm("Inicializacao concluída.\nDeseja fechar a aplicação"))
            { System.Environment.Exit(0); }

         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
         finally { this.IsBusy = false; }
      }
      #endregion      

   }
}