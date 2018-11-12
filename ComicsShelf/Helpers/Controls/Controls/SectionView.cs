using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class SectionView : StackLayout
   {

      #region New
      public SectionView()
      {

         this.Margin = new Thickness(0, 0, 0, 20);
         this.Padding = 0;
         this.Spacing = 0;

         this.Label = new Label
         {
            Margin = new Thickness(15, 0),
            TextColor = Color.Accent, 
            FontAttributes = FontAttributes.Bold,
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), 
            IsVisible = false
         };
         this.Children.Add(this.Label);

         this.Items = new FlexLayout
         {
            Direction = FlexDirection.Row,
            AlignItems = FlexAlignItems.Start,
            AlignContent = FlexAlignContent.Start,
            JustifyContent = FlexJustify.Start,
            Wrap = FlexWrap.Wrap
         };
         this.Children.Add(this.Items);

      }
      #endregion


      #region Title
      Label Label { get; set; }
      public static readonly BindableProperty TitleProperty =
         BindableProperty.Create("Title", typeof(string), typeof(SectionView), string.Empty,
         propertyChanged: OnTitleChanged);
      public string Title
      {
         get { return (string)GetValue(TitleProperty); }
         set { SetValue(TitleProperty, value); }
      }
      private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
      {
         var self = (bindable as SectionView).Label;
         self.IsVisible = !string.IsNullOrEmpty((string)newValue);
         self.Text = (string)newValue;
      }
      #endregion


      #region Items
      FlexLayout Items { get; set; }
      public DataTemplate ItemTemplate { get; set; }
      #endregion

      #region ItemsSource
      public static readonly BindableProperty ItemsSourceProperty =
         BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(SectionView), null,
         propertyChanged: OnItemsSourceChanged);
      public IList ItemsSource
      {
         get { return (IList)GetValue(ItemsSourceProperty); }
         set { SetValue(ItemsSourceProperty, value); }
      }
      private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
      { (bindable as SectionView).ItemsObserve(); }
      #endregion

      #region ItemsObserve
      private void ItemsObserve()
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
            this.Items.Children.Clear();

            // LOAD CURRENT ITEMS LIST
            var itemsList = ((IEnumerable<object>)this.ItemsSource).ToList();
            if (itemsList == null || itemsList.Count == 0) { return; }

            // LOOP THROUGH ITEMS
            foreach (var item in itemsList)
            {

               // CREATE A VIEW USING THE TEMPLATE
               var itemView = (View)this.ItemTemplate.CreateContent();
               itemView.InputTransparent = false;
               itemView.BindingContext = item;

               // ATTACH GESTURE
               itemView.GestureRecognizers.Add(new TapGestureRecognizer
               {
                  Command = this.ItemTappedTransition,
                  CommandParameter = itemView,
                  NumberOfTapsRequired = 1
               });

               this.Items.Children.Add(itemView);
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
               await itemView.FadeTo(1.0, 300, Easing.SinIn);
               await System.Threading.Tasks.Task.Run(() => Device.BeginInvokeOnMainThread(() => this.ItemTappedCommand?.Execute(itemView.BindingContext)));
            });
         }
      }
      #endregion

      #region ItemTappedCommand
      public static readonly BindableProperty ItemTappedCommandProperty =
         BindableProperty.Create("ItemTappedCommand", typeof(ICommand), typeof(SectionView), null);
      public ICommand ItemTappedCommand
      {
         get { return (ICommand)GetValue(ItemTappedCommandProperty); }
         set { SetValue(ItemTappedCommandProperty, value); }
      }
      #endregion

   }
}