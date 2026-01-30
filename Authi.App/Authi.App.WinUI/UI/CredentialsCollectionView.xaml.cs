using Authi.App.Logic.ViewModels;

namespace Authi.App.WinUI.UI
{
    public sealed partial class CredentialsCollectionView
    {
        public ICredentialsCollectionViewModel? ViewModel { get; set; }

        public CredentialsCollectionView()
        {
            InitializeComponent();
        }
    }
}
