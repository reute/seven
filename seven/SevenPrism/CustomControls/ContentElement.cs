using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace SevenPrism.CustomControls
{
    [ContentProperty(nameof(Content))]
    public class ContentElement : FrameworkElement
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(FrameworkContentElement), typeof(ContentElement), new UIPropertyMetadata(null));

        public FrameworkContentElement Content
        {
            get => (FrameworkContentElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }
}
