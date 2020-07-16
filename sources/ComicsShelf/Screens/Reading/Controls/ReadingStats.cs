using System;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Reading
{
   public class ReadingStats : StackLayout
   {

      public ReadingStats()
      {
         var eventTrigger = new EventTrigger { Event = "OnReadingPageChanged" };
         eventTrigger.Actions.Add(new Controls.Animations.FadeAnimation());
         this.Triggers.Add(eventTrigger);
         ShowCommand = new Command(() => OnReadingPageChanged?.Invoke(this, EventArgs.Empty));
      }

      public Command ShowCommand { get; set; }

      public event EventHandler OnReadingPageChanged;

      public static readonly BindableProperty ReadingPageProperty =
         BindableProperty.Create("ReadingPage", typeof(short?), typeof(ReadingStats), null,
         propertyChanged: OnReadingPageChanging);
      public short? ReadingPage
      {
         get { return (short?)GetValue(ReadingPageProperty); }
         set { SetValue(ReadingPageProperty, value); }
      }

      private static void OnReadingPageChanging(BindableObject bindable, object oldValue, object newValue)
      { (bindable as ReadingStats).OnReadingPageChanged?.Invoke(bindable, EventArgs.Empty); }

   }
}
