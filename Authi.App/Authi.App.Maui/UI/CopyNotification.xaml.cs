using Authi.App.Logic.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Authi.App.Maui.UI;

public partial class CopyNotification
{
    private CancellationTokenSource _ctx;
    private ICopyNotificationViewModel _viewModel;

    public CopyNotification()
    {
        InitializeComponent();
    }

    private async void OnTotpCopied()
    {
        if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
        {
            return;
        }

        _ctx?.Cancel();
        Content.CancelAnimations();

        if (_ctx != null)
        {
            Content.Opacity = 1;
            Content.TranslationY = 0;
        }
        else
        {
            new Animation(value =>
            {
                Content.Opacity = value;
                Content.TranslationY = (1 - value) * 180;
            }, 0, 1)
                .Commit(
                    Content,
                    "CopyNotificationSlideIn",
                    length: AnimationLength.DefaultUnsigned,
                    easing: Easing.CubicOut);
        }

        var ctx = _ctx = new CancellationTokenSource();
        await Task.Delay(TimeSpan.FromMilliseconds(AnimationLength.Second));

        if (ctx.IsCancellationRequested)
        {
            return;
        }
        new Animation(value =>
        {
            if (ctx.IsCancellationRequested)
            {
                return;
            }

            Content.Opacity = value;
            Content.TranslationY = (1 - value) * 180;
        }, 1, 0)
            .Commit(
                Content,
                "CopyNotificationSlideOut",
                length: AnimationLength.DefaultUnsigned,
                easing: Easing.CubicIn,
                finished: (value, isCancelled) =>
                {
                    if (ctx.IsCancellationRequested)
                    {
                        return;
                    }

                    _ctx = null;
                });
    }

    private void OnBindingContextChanged(object sender, EventArgs e)
    {
        var oldViewModel = _viewModel;
        _viewModel = BindingContext as ICopyNotificationViewModel;
        OnViewModelChanged(oldViewModel, _viewModel);
    }

    private void OnViewModelChanged(ICopyNotificationViewModel oldViewModel, ICopyNotificationViewModel newViewModel)
    {
        if (oldViewModel != null)
        {
            oldViewModel.TotpCopied -= OnTotpCopied;
        }
        if (newViewModel != null)
        {
            newViewModel.TotpCopied += OnTotpCopied;
        }
    }
}