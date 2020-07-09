using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicsShelf.Controls
{

   [ContentProperty("Text")]
   public class TranslateExtension : IMarkupExtension
   {

      public string Text { get; set; }

      public object ProvideValue(IServiceProvider serviceProvider)
      {
         if (Text == null) return "";
         var translatedText = Resources.Translations.ResourceManager.GetString(Text);
         if (string.IsNullOrEmpty(translatedText))
            translatedText = Text;
         return translatedText;
      }

   }

}
