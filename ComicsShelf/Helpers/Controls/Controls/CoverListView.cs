using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class CoverListView: FlexLayout
   {

      #region New
      public CoverListView()
      {
         this.Direction = FlexDirection.Row;
         this.AlignItems = FlexAlignItems.Start;
         this.AlignContent = FlexAlignContent.Start;
         this.JustifyContent = FlexJustify.Start;
         this.Wrap = FlexWrap.Wrap;
      }
      #endregion


      #region ItemTemplate
      public DataTemplate ItemTemplate { get; set; }
      #endregion  

      #region ItemsSource
      public static readonly BindableProperty ItemsSourceProperty =
         BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(ListView), null,
            BindingMode.TwoWay, propertyChanged: OnItemsSourceChanged);
      public IList ItemsSource
      {
         get { return (IList)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }
      private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as CoverListView)?.ItemsRefreshObserve(); }
      #endregion

      #region ItemsRefreshObserve
      private void ItemsRefreshObserve()
      {
         this.ItemsRefresh();

         var itemsSource = (this.ItemsSource as Observables.INotifyObservableCollectionChanged);
         if (itemsSource != null) {
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
            this.Children?.Clear();

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

               // ATTACH GESTURE
               itemView?.GestureRecognizers.Add(new TapGestureRecognizer {
                  Command = this.ItemTappedTransition,
                  CommandParameter = itemView,
                  NumberOfTapsRequired = 1
               });

               this.Children?.Add(itemView);
            }

         }
         catch (Exception ex) { throw; }
      }
      #endregion


      #region ItemTappedTransition
      private ICommand ItemTappedTransition
      {
         get
         {
            return new Command(async (commandParameter) =>
            {
               var itemView = (View)commandParameter;
               await itemView.FadeTo(0.5, 100, Easing.SinOut);
               await itemView.FadeTo(1.0, 400, Easing.SinIn);
               await System.Threading.Tasks.Task.Run(() => Xamarin.Forms.Device.BeginInvokeOnMainThread(() => this.ItemTappedCommand?.Execute(itemView.BindingContext)));
            });
         }
      }
      #endregion

      #region ItemTappedCommand
      public static readonly BindableProperty ItemTappedCommandProperty =
         BindableProperty.Create("ItemTappedCommand", typeof(ICommand), typeof(CoverListView), null);
      public ICommand ItemTappedCommand
      {
         get { return (ICommand)GetValue(ItemTappedCommandProperty); }
         set { SetValue(ItemTappedCommandProperty, value); }
      }
      #endregion

   }
}