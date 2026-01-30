using Authi.App.Logic.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace Authi.App.Maui.UI;

public partial class MenuBarView : IAdaptiveView
{
    public IMenuBarViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            OnViewModelChanged(_viewModel, value);
            _viewModel = value;
        }
    }

    private IMenuBarViewModel _viewModel;

    public MenuBarView()
    {
        InitializeComponent();
    }

    private async void OnLoaded(object sender, EventArgs e)
    {
        await Task.Delay(AnimationLength.Long);
        await ProgressBar.FadeTo(1, AnimationLength.DefaultUnsigned, Easing.CubicOut);
    }

    private void OnViewModelChanged(IMenuBarViewModel oldViewModel, IMenuBarViewModel newViewModel)
    {
        BindingContext = newViewModel;
    }

    private void OnSettingsClicked(object sender, EventArgs e)
    {
        _viewModel?.ShowSettings();
    }

    public void SetCompactSize(bool isCompact)
    {
        Padding = new Thickness(0, MauiApp.Current.SystemInsets.Top, 0, 0);
    }
}