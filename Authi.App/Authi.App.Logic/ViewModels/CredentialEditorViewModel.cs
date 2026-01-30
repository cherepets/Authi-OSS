using Authi.App.Logic.Data;
using Authi.Common.Client;
using Authi.Common.Extensions;
using System;
using System.ComponentModel;

namespace Authi.App.Logic.ViewModels
{
    public interface ICredentialEditorViewModel : INotifyPropertyChanged
    {
        string PageTitle { get; }
        string Title { get; set; }
        string Secret { get; set; }
        bool CanSave { get; }
        void QrScanned(string code);
        void Save();
        void Close();
    }

    public abstract class CredentialEditorViewModelBase : ViewModelBase, ICredentialEditorViewModel
    {
        public abstract string PageTitle { get; }

        public string Title
        {
            get => Get<string>() ?? string.Empty;
            set
            {
                Set(value);
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public string Secret
        {
            get => Get<string>() ?? string.Empty;
            set
            {
                Set(value);
                OnPropertyChanged(nameof(CanSave));
            }
        }

        public bool CanSave => !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Secret);

        internal Credential Model { get; private set; }

        protected CredentialEditorViewModelBase(Credential dto)
        {
            Model = dto;
            UpdateModel(dto);
        }

        public abstract void Save();

        public void QrScanned(string code)
        {
            try
            {
                var uri = new OtpauthUri(code);

                Title = uri.Issuer;
                Secret = uri.Secret;
            }
            catch (Exception exception)
            {
                Services.Logger.Write(exception);
            }
        }

        public void Close()
        {
            Services.Messenger.NavigationPop.Publish(this);
        }

        internal void UpdateModel(Credential dto)
        {
            Model = dto;
            Model.MapPropertiesTo(this);
        }
    }
}
