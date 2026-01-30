using Authi.App.Logic.ViewModels;
using Authi.App.WinUI.Extensions;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Numerics;
using Windows.System;

namespace Authi.App.WinUI.UI
{
    public sealed partial class CredentialItemView
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(CredentialViewModel), typeof(CredentialItemView), null);

        public CredentialViewModel? ViewModel
        {
            get => (CredentialViewModel?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(CredentialItemView), new PropertyMetadata(false, OnIsEditingChanged));

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }

        private bool _focusGoesOutside;

        public CredentialItemView()
        {
            InitializeComponent();
            MainButton.Translation = new Vector3(0, 0, 8);
        }

        private static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CredentialItemView)d;
            if ((bool)e.NewValue)
            {
                control.OpenEditingMode();
            }
            else
            {
                control.HideEditingMode();
            }
        }

        private void OnMainButtonClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel is CredentialViewModel viewModel)
            {
                if (viewModel.IsEditing)
                {
                    viewModel.IsEditing = false;
                }
                viewModel.CopyToClipboard();
            }
        }

        private void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (ViewModel is CredentialViewModel viewModel &&
                (e.PointerDeviceType == PointerDeviceType.Mouse || e.PointerDeviceType == PointerDeviceType.Touchpad))
            {
                viewModel.IsEditing = !viewModel.IsEditing;
            }
        }

        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (ViewModel is CredentialViewModel viewModel &&
                e.HoldingState == HoldingState.Started &&
                (e.PointerDeviceType == PointerDeviceType.Touch || e.PointerDeviceType == PointerDeviceType.Pen))
            {
                viewModel.IsEditing = !viewModel.IsEditing;
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel is CredentialViewModel viewModel &&
                viewModel.IsEditing &&
                _focusGoesOutside)
            {
                viewModel.IsEditing = false;
            }
        }

        private void OnLosingFocus(UIElement sender, LosingFocusEventArgs args)
        {
            _focusGoesOutside =
                args.NewFocusedElement != this &&
                args.NewFocusedElement != MainButton &&
                args.NewFocusedElement != EditButton &&
                args.NewFocusedElement != DeleteButton;
        }

        private void OpenEditingMode()
        {
            Animator
                .NonBlocking()
                .OnStart(() =>
                {
                    var currentFocus = FocusManager.GetFocusedElement() as UIElement;
                    EditButton.Visibility = Visibility.Visible;
                    DeleteButton.Visibility = Visibility.Visible;
                    EditButton.Focus(currentFocus?.FocusState ?? FocusState.Programmatic);

                    MainButton.IsEnabled = false;
                })
                .From(MainButton, Animator.Property.TranslateX, 0)
                .From(EditButton, Animator.Property.ScaleX, 0)
                .From(EditButton, Animator.Property.ScaleY, 0)
                .From(DeleteButton, Animator.Property.ScaleX, 0)
                .From(DeleteButton, Animator.Property.ScaleY, 0)
                .To(MainButton, Animator.Property.TranslateX, -112, Animator.Length.Default, Animator.Easing.Out)
                .To(EditButton, Animator.Property.ScaleX, 1, Animator.Length.Default, Animator.Easing.In)
                .To(EditButton, Animator.Property.ScaleY, 1, Animator.Length.Default, Animator.Easing.In)
                .To(DeleteButton, Animator.Property.ScaleX, 1, Animator.Length.Default, Animator.Easing.In)
                .To(DeleteButton, Animator.Property.ScaleY, 1, Animator.Length.Default, Animator.Easing.In)
                .Run();
        }

        private void HideEditingMode()
        {
            Animator
                .NonBlocking()
                .From(MainButton, Animator.Property.TranslateX, -112)
                .From(EditButton, Animator.Property.ScaleX, 1)
                .From(EditButton, Animator.Property.ScaleY, 1)
                .From(DeleteButton, Animator.Property.ScaleX, 1)
                .From(DeleteButton, Animator.Property.ScaleY, 1)
                .To(MainButton, Animator.Property.TranslateX, 0, Animator.Length.Default, Animator.Easing.In)
                .To(EditButton, Animator.Property.ScaleX, 0, Animator.Length.Default, Animator.Easing.Out)
                .To(EditButton, Animator.Property.ScaleY, 0, Animator.Length.Default, Animator.Easing.Out)
                .To(DeleteButton, Animator.Property.ScaleX, 0, Animator.Length.Default, Animator.Easing.Out)
                .To(DeleteButton, Animator.Property.ScaleY, 0, Animator.Length.Default, Animator.Easing.Out)
                .OnFinish(() =>
                {
                    var currentFocus = FocusManager.GetFocusedElement() as UIElement;
                    MainButton.IsEnabled = true;
                    if (!_focusGoesOutside)
                    {
                        MainButton.Focus(currentFocus?.FocusState ?? FocusState.Programmatic);
                    }

                    EditButton.Visibility = Visibility.Collapsed;
                    DeleteButton.Visibility = Visibility.Collapsed;
                })
                .Run();
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (ViewModel is CredentialViewModel viewModel)
            {
                if (!viewModel.IsEditing && (e.Key == VirtualKey.Application || e.Key == VirtualKey.Left))
                {
                    viewModel.IsEditing = true;
                }
                else if (viewModel.IsEditing)
                {
                    if (e.Key == VirtualKey.Application || e.Key == VirtualKey.Escape)
                    {
                        viewModel.IsEditing = false;
                    }
                    else if (e.Key == VirtualKey.Left && DeleteButton.FocusState != FocusState.Unfocused)
                    {
                        EditButton.Focus(DeleteButton.FocusState);
                    }
                    else if (e.Key == VirtualKey.Right && EditButton.FocusState != FocusState.Unfocused)
                    {
                        DeleteButton.Focus(EditButton.FocusState);
                    }
                }
            }
        }
    }
}
