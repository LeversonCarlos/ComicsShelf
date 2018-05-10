using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Tools
{
   public class ToolsVM : Helpers.ViewModels.DataVM<ToolsData>
   {

      #region New
      public ToolsVM()
      {
         this.Title = R.Strings.TOOLS_PAGE_TITLE; ;
         this.ViewType = typeof(ToolsPage);
         this.Data = new ToolsData { LibraryPath = App.Settings.Paths.LibraryPath };
         this.LibraryTappedCommand = new Command(async (item) => await this.LibraryTapped(item));
         MessagingCenter.Subscribe<Engine.StepData>(this, Engine.StepData.KEY, (data) =>
         {
            this.Data.Text = data.Text;
            this.Data.Details = data.Details;
            this.Data.Progress = data.Progress;
         });
      }
      #endregion

      #region LibraryTapped
      public Command LibraryTappedCommand { get; set; }
      private async Task LibraryTapped(object item)
      {
         try
         {

            if (await Engine.LibraryEngine.Execute())
            { await Helpers.ViewModels.NavVM.PopAsync(); }

         }
         catch (Exception ex) { await App.Message.Show(ex.ToString()); }
      }
      #endregion

   }
}