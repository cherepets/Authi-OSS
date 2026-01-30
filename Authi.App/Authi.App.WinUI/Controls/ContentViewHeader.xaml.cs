using Authi.App.WinUI.UI;
using System;
using Microsoft.UI.Xaml;

namespace Authi.App.WinUI.Controls
{
    public sealed partial class ContentViewHeader : IAdaptiveView
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ContentViewHeader), new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public event EventHandler<RoutedEventArgs>? CloseRequested;

        public ContentViewHeader()
        {
            InitializeComponent();
        }

        public void SetCompactSize(bool isCompact)
        {
            BackButton.Visibility = isCompact ? Visibility.Visible : Visibility.Collapsed;
            CloseButton.Visibility = isCompact ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, e);
        }
    }
}
