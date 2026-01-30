using Authi.App.Logic.ViewModels;
using Microsoft.Maui;
using System;

namespace Authi.App.Maui.UI;

public partial class CredentialsCollectionView : IAdaptiveView
{
    public ICredentialsCollectionViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            OnViewModelChanged(_viewModel, value);
            _viewModel = value;
        }
    }

    private ICredentialsCollectionViewModel _viewModel;

    public CredentialsCollectionView()
    {
        InitializeComponent();
    }

    private void OnAddCredentialsClicked(object sender, EventArgs e)
    {
        _viewModel.ShowAddCredentials();
    }

    private void OnViewModelChanged(ICredentialsCollectionViewModel oldViewModel, ICredentialsCollectionViewModel newViewModel)
    {
        BindingContext = newViewModel;
    }

    public void SetCompactSize(bool isCompact)
    {
        AddCredentialsButton.Margin = new Thickness(24, 24, 24, 24 + MauiApp.Current.SystemInsets.Bottom);
        ScrollView.Padding = new Thickness(0, 0, 0, AddCredentialsButton.HeightRequest + AddCredentialsButton.Margin.VerticalThickness);
    }
}