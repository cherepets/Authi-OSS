using Authi.App.Logic.ViewModels;
using Authi.App.WinUI.Extensions;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Authi.App.WinUI.UI
{
    public sealed partial class CopyNotification
    {
        private const double HiddenTranslateY = 180;

        public ICopyNotificationViewModel? ViewModel
        {
            get => _viewModel;
            set
            {
                OnViewModelChanged(_viewModel, value);
                _viewModel = value;
            }
        }

        private ICopyNotificationViewModel? _viewModel;

        private CancellationTokenSource? _ctx;

        public CopyNotification()
        {
            InitializeComponent();
            Animator
                .NonBlocking()
                .From(Content, Animator.Property.TranslateY, HiddenTranslateY)
                .OnFinish(() => Border.Translation = new Vector3(0, 0, 16))
                .Run();
        }

        private void OnViewModelChanged(ICopyNotificationViewModel? oldViewModel, ICopyNotificationViewModel? newViewModel)
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

        private async void OnTotpCopied()
        {
            _ctx?.Cancel();

            if (_ctx != null)
            {
                await Animator
                    .NonBlocking()
                    .From(Content, Animator.Property.TranslateY, 0)
                    .RunAsync();
            }
            else
            {
                await Animator
                    .NonBlocking()
                    .From(Content, Animator.Property.TranslateY, HiddenTranslateY)
                    .To(Content, Animator.Property.TranslateY, 0, Animator.Length.Default, Animator.Easing.Out)
                    .RunAsync();
            }

            var ctx = _ctx = new CancellationTokenSource();
            await Task.Delay(Animator.Length.Second);

            if (ctx.IsCancellationRequested)
            {
                return;
            }

            await Animator
                .NonBlocking()
                    .From(Content, Animator.Property.TranslateY, 0)
                    .To(Content, Animator.Property.TranslateY, HiddenTranslateY, Animator.Length.Default, Animator.Easing.In)
                    .OnFinish(() =>
                    {
                        if (ctx.IsCancellationRequested)
                        {
                            return;
                        }

                        _ctx = null;
                    })
                    .RunAsync(ctx.Token);
        }
    }
}
