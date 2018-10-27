using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class RepeaterView : StackLayout
   {

      #region ItemTemplate
      public DataTemplate ItemTemplate { get; set; }
      #endregion  

      #region ItemsSource
      public static readonly BindableProperty ItemsSourceProperty =
         BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(RepeaterView), null,
            BindingMode.TwoWay, propertyChanged: OnItemsSourceChanged);
      public IList ItemsSource
      {
         get { return (IList)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }
      private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as RepeaterView)?.ItemsRefreshObserve(); }
      #endregion

      #region ItemsRefreshObserve
      private void ItemsRefreshObserve()
      {
         this.ItemsRefresh();

         var itemsSource = (this.ItemsSource as Observables.INotifyObservableCollectionChanged);
         if (itemsSource != null)
         {
            itemsSource.ObservableCollectionChanged +=
               (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => { this.ItemsRefresh(); };
         }

      }
      #endregion

      #region ItemsRefresh
      private void ItemsRefresh()
      {
         try
         {

            // INITIALIZE
            this.Children.Clear();

            // LOAD CURRENT ITEMS LIST
            var itemsList = ((IEnumerable<object>)this.ItemsSource).ToList();
            if (itemsList == null || itemsList.Count == 0) { return; }

            // LOOP THROUGH ITEMS
            foreach (var item in itemsList)
            {

               // CREATE A VIEW USING THE TEMPLATE
               var itemView = (View)this.ItemTemplate.CreateContent();
               itemView.InputTransparent = false;

               // BIND CONTEXT
               var bindableObject = (itemView as BindableObject);
               if (bindableObject != null)
               { bindableObject.BindingContext = item; }

               this.Children.Add(itemView);
            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion

   }
}