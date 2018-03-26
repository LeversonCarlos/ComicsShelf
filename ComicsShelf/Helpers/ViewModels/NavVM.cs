using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.ViewModels
{
   public class NavVM : BaseVM
   {

      #region ViewType
      Type _ViewType = null;
      public Type ViewType
      {
         get { return this._ViewType; }
         set { this.SetProperty(ref this._ViewType, value); }
      }
      #endregion


      #region PushAsync

      public static async Task PushAsync<T>(params object[] args) where T : NavVM
      { await PushAsync<T>(false, args); }

      public static async Task PushAsync<T>(bool popAll, params object[] args) where T : NavVM
      {
         try
         {
            var viewModelType = typeof(T);
            var viewModel = Activator.CreateInstance(viewModelType, args) as T;
            if (viewModel == null) { throw new Exception("Cannot create view model instance."); }

            await PushAsync(viewModel, popAll);
         }
         catch (Exception ex) { throw; }
      }

      private static async Task PushAsync(NavVM viewModel, bool popAll)
      {
         try
         {

            // VIEW
            var viewType = viewModel.ViewType;
            var view = Activator.CreateInstance(viewType) as Page;
            if (view == null) { throw new Exception("Cannot create view instance."); }
            view.BindingContext = viewModel;

            // NAVIGATION
            var mainPage = Application.Current.MainPage as Page;
            var navigation = mainPage.Navigation;
            if (popAll && navigation.NavigationStack.Count > 0)
            {
               navigation.InsertPageBefore(view, navigation.NavigationStack.FirstOrDefault());
               await navigation.PopToRootAsync();
            }
            else { await navigation.PushAsync(view); }
            // if (Device.Idiom == TargetIdiom.Phone) { mainPage.IsPresented = false; }

         }
         catch (Exception ex) { throw; }
      }

      #endregion

      #region PopAsync
      public static async Task PopAsync()
      {
         var mainPage = Application.Current.MainPage as Page;
         var navigation = mainPage.Navigation;
         await navigation.PopToRootAsync();
      }
      #endregion


      #region Dispose
      public override void Dispose()
      {
         base.Dispose();
         this.ViewType = null;
      }
      #endregion

   }
}