using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Controls
{
   public class RepeaterView : StackLayout
   {

      #region ItemTemplate
      public DataTemplate ItemTemplate { get; set; }
      #endregion  

      #region ItemsSource
      public static readonly BindableProperty ItemsSourceProperty =
         BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(RepeaterView), null,
         propertyChanged: OnItemsSourceChanged);
      public IList ItemsSource
      {
         get { return (IList)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }
      private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as RepeaterView).ItemsObserve(); }
      #endregion

      #region ItemsObserve
      private void ItemsObserve()
      {

         var resetParam = new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset);
         this.ItemsRefresh(resetParam);

         var itemsSource = (this.ItemsSource as System.Collections.Specialized.INotifyCollectionChanged);
         if (itemsSource != null)
         {
            itemsSource.CollectionChanged +=
               (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
               {
                  Device.BeginInvokeOnMainThread(() => this.ItemsRefresh(e));
               };
         }

      }
      #endregion

      #region ItemsRefresh
      private void ItemsRefresh(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
      {
         try
         {

            // RESET
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
               this.Children.Clear();

               // LOAD CURRENT ITEMS LIST
               if (this.ItemsSource == null) { return; }
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

                  // ATTACH GESTURE
                  itemView.GestureRecognizers.Add(new TapGestureRecognizer
                  {
                     Command = new Command(() => { this.ItemTappedCommand?.Execute(item); }),
                     CommandParameter = item,
                     NumberOfTapsRequired = 1
                  });

                  this.Children.Add(itemView);
               }
               return;
            }

            // REMOVE ACTION
            if (e.OldItems != null && e.OldItems.Count != 0 && e.OldStartingIndex != -1)
            {
               for (int i = 0; i < e.OldItems.Count; i++)
               {
                  var view = this.Children[i];
                  view.BindingContext = null;
                  this.Children.RemoveAt(e.OldStartingIndex);
               }
            }

            // ADD ACTION
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
               var itemIndex = e.NewStartingIndex;
               foreach (var item in e.NewItems)
               {

                  // CREATE A VIEW USING THE TEMPLATE
                  var itemView = (View)this.ItemTemplate.CreateContent();
                  itemView.InputTransparent = false;

                  // BIND CONTEXT
                  var bindableObject = (itemView as BindableObject);
                  if (bindableObject != null)
                  { bindableObject.BindingContext = item; }

                  // ATTACH GESTURE
                  itemView.GestureRecognizers.Add(new TapGestureRecognizer
                  {
                     Command = new Command(() => { this.ItemTappedCommand?.Execute(item); }),
                     CommandParameter = item,
                     NumberOfTapsRequired = 1
                  });

                  // ADD
                  if (e.NewStartingIndex == -1) { this.Children.Add(itemView); }
                  else
                  {
                     this.Children.Insert(itemIndex, itemView);
                     itemIndex++;
                  }

               }
            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion

      #region ItemTappedCommand
      public static readonly BindableProperty ItemTappedCommandProperty =
         BindableProperty.Create(nameof(ItemTappedCommand), typeof(ICommand), typeof(RepeaterView), null);
      public ICommand ItemTappedCommand
      {
         get { return (ICommand)GetValue(ItemTappedCommandProperty); }
         set { SetValue(ItemTappedCommandProperty, value); }
      }
      #endregion

   }
}