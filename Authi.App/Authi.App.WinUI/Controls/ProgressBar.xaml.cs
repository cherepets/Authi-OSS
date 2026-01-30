using Authi.App.Logic;
using Authi.App.WinUI.Extensions;
using System;
using System.Threading;

namespace Authi.App.WinUI.Controls
{
    public sealed partial class ProgressBar
    {
        private CancellationTokenSource? _ctx;

        public ProgressBar()
        {
            InitializeComponent();
        }

        public async void UpdateTime(TimeSpan time)
        {
            _ctx?.Cancel();
            _ctx = new CancellationTokenSource();
            var progress = 1d - time.TotalMilliseconds / Config.UpdateMs;
            await Animator
                .NonBlocking()
                .From(Content, Animator.Property.ScaleX, progress)
                .To(Content, Animator.Property.ScaleX, 1, time, null)
                .RunAsync(_ctx.Token);
        }
    }
}
