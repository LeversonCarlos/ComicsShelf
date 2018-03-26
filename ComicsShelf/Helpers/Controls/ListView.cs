using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class ListView : Xamarin.Forms.ListView
   {

      #region New

      public ListView() : this(ListViewCachingStrategy.RecycleElement) { }

      public ListView(ListViewCachingStrategy strategy) : base(strategy)
      { this.ItemSelected += OnItemSelected; }

      #endregion

      #region OnItemSelected
      private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
      {
         if (this.ItemTappedCommand == null) { return; }
         if (this.ItemTappedCommand.CanExecute(e.SelectedItem) == false) { return; }
         this.ItemTappedCommand.Execute(e.SelectedItem);
      }
      #endregion

      #region ItemTappedCommand

      public static readonly BindableProperty ItemTappedCommandProperty =
         BindableProperty.Create("ItemTappedCommand", typeof(ICommand), typeof(ListView), null);

      public ICommand ItemTappedCommand
      {
         get { return (ICommand)GetValue(ItemTappedCommandProperty); }
         set { SetValue(ItemTappedCommandProperty, value); }
      }

      #endregion

   }
}