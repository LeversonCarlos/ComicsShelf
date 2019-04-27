using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   public class SectionView : StackLayout
   {

      #region New
      public SectionView()
      {

         this.Margin = new Thickness(0, 0, 0, 5);
         this.Padding = 0;
         this.Spacing = 0;

         this.Label = new Label
         {
            Margin = new Thickness(10, 0),
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
            Wrap = FlexWrap.Wrap,
            Margin = new Thickness(10, 0, 0, 0)
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

         var itemsSource = (this.ItemsSource as INotifyCollectionChanged);
         if (itemsSource != null)
         {
            itemsSource.CollectionChanged +=
               (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => {
                  Device.BeginInvokeOnMainThread(this.ItemsRefresh);
               };
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
            if (this.ItemsSource == null) { return; }
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

               await Task.WhenAll(
                  itemView.FadeTo(0.75, 150, Easing.SpringOut),
                  itemView.ScaleTo(0.75, 150, Easing.SpringOut)
               );
               await Task.WhenAll(
                  itemView.ScaleTo(1.00, 100, Easing.SpringOut),
                  itemView.FadeTo(1.00, 100, Easing.SpringOut)
               );
               Device.BeginInvokeOnMainThread(() => this.ItemTappedCommand.Execute(itemView.BindingContext));

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