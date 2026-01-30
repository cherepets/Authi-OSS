using Authi.Common.Services;
using System;
using System.Threading.Tasks;

namespace Authi.App.Logic.Services
{
    [Service]
    public interface IDialogManager
    {
        Task ShowDialogAsync(string title, string message, string? primaryButtonText = null, string? cancelButtonText = null, Action? onPrimary = null, Action? onCancel = null);
    }

    public class DialogManager : IDialogManager
    {
        private static IDialogManager? _impl;

        public Task ShowDialogAsync(string title, string message, string? primaryButtonText = null, string? cancelButtonText = null, Action? onPrimary = null, Action? onCancel = null)
            => _impl?.ShowDialogAsync(title, message, primaryButtonText, cancelButtonText, onPrimary, onCancel) ?? Task.CompletedTask;

        public static void Register(IDialogManager impl)
        {
            _impl = impl;
        }
    }
}
