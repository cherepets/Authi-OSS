using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Authi.App.WinUI.Controls
{
    public sealed partial class SettingsSectionButton : UserControl
    {
        public string? Icon { get; set; }
        public string? Title { get; set; }

        public string? Caption { get; set; }

        public event Action? Click;

        public SettingsSectionButton()
        {
            InitializeComponent();
        }

        private void OnClicked(object sender, RoutedEventArgs e)
        {
            Click?.Invoke();
        }
    }
}
