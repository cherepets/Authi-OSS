using Authi.App.Logic.Data;
using Authi.App.Logic.Services;
using Authi.App.Logic.ViewModels;
using Authi.App.Test.Mocks;
using Authi.Common.Services;
using Authi.Common.Test.Mocks;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Authi.App.Test
{
    [TestClass]
    public class CredentialViewModelTests : AppTestsBase
    {
        private static ObjectId LocalId => MockCredentialStorage.CreateLocalId(1);

        private const string Title1 = "Title 1";
        private const string Title2 = "Title 2";
        private const string Secret1 = "AEAQCAIBAEAQCAIB";
        private const string Secret2 = "AIBAEAQCAIBAEAQC";
        private const string Totp1 = "123456";
        private const string Totp2 = "987654";
        private const string DisplayTotp1 = "123 456";
        private const string DisplayTotp2 = "987 654";
        private const long Timestamp1 = 1;
        private const long Timestamp2 = 2;

        private MockClock MockClock => (MockClock)Services.Clock;

        [TestMethod]
        public void CredentialUpdateTest()
        {
            ConfigureServices();

            var dto1 = new Credential
            {
                Title = Title1,
                Secret = Secret1
            };
            var credentialVM = new CredentialViewModel(dto1);

            Assert.AreEqual(Title1, credentialVM.Title);
            Assert.AreEqual(Secret1, credentialVM.Secret);
            Assert.AreEqual(Totp1, credentialVM.Totp);
            Assert.AreEqual(DisplayTotp1, credentialVM.DisplayTotp);

            var dto2 = new Credential
            {
                Title = Title2,
                Secret = Secret2
            };
            credentialVM.UpdateModel(dto2);

            Assert.AreEqual(Title2, credentialVM.Title);
            Assert.AreEqual(Secret2, credentialVM.Secret);
            Assert.AreEqual(Totp2, credentialVM.Totp);
            Assert.AreEqual(DisplayTotp2, credentialVM.DisplayTotp);
        }

        [TestMethod]
        public async Task AddCredentialTest()
        {
            ConfigureServices();

            var credentialCollection = new ObservableCollection<CredentialViewModel>();
            var credentialVM = new AddCredentialViewModel(credentialCollection)
            {
                Title = Title1,
                Secret = Secret1
            };
            credentialVM.Save();
            var addedCredential = credentialCollection.Single();

            Assert.AreEqual(Title1, addedCredential.Title);
            Assert.AreEqual(Secret1, addedCredential.Secret);
            Assert.AreEqual(Totp1, addedCredential.Totp);
            Assert.AreEqual(DisplayTotp1, addedCredential.DisplayTotp);

            var local = await Services.LocalCredentialStorage.GetAllAsync();
            var storageCredential = local.Single();
            Assert.AreEqual(Title1, storageCredential.Title);
            Assert.AreEqual(Secret1, storageCredential.Secret);
            Assert.AreEqual(Timestamp1, storageCredential.Timestamp);
        }

        [TestMethod]
        public async Task EditCredentialTest()
        {
            ConfigureServices();

            var dto1 = new Credential
            {
                LocalId = LocalId,
                Title = Title1,
                Secret = Secret1
            };
            var credentialVM = new CredentialViewModel(dto1);
            await Services.LocalCredentialStorage.InsertAsync(dto1);

            ViewModelBase? contentVM = null;
            void OnContentChanged(object? sender, ViewModelBase vm) { contentVM = vm; }
            Services.Messenger.NavigationPush.Subscribe += OnContentChanged;
            credentialVM.Edit();
            Services.Messenger.NavigationPush.Subscribe -= OnContentChanged;

            Assert.IsNotNull(contentVM);
            var editCredentialVM = (EditCredentialViewModel)contentVM;
            Assert.AreEqual(Title1, editCredentialVM.Title);
            Assert.AreEqual(Secret1, editCredentialVM.Secret);

            editCredentialVM.Title = Title2;
            editCredentialVM.Secret = Secret2;

            MockClock.UniversalTime = DateTimeOffset.UnixEpoch.AddMilliseconds(Timestamp2);
            editCredentialVM.Save();

            var local = await Services.LocalCredentialStorage.GetAllAsync();
            var storageCredential = local.Single();
            Assert.AreEqual(Title2, storageCredential.Title);
            Assert.AreEqual(Secret2, storageCredential.Secret);
            Assert.AreEqual(Timestamp2, storageCredential.Timestamp);

            Assert.AreEqual(Title2, credentialVM.Title);
            Assert.AreEqual(Secret2, credentialVM.Secret);
            Assert.AreEqual(Totp2, credentialVM.Totp);
            Assert.AreEqual(DisplayTotp2, credentialVM.DisplayTotp);
        }

        [TestMethod]
        public async Task DeleteCredentialTest()
        {
            ConfigureServices();

            var dto1 = new Credential
            {
                LocalId = LocalId,
                Title = Title1,
                Secret = Secret1
            };
            var credentialVM = new CredentialViewModel(dto1);
            await Services.LocalCredentialStorage.InsertAsync(dto1);

            CredentialViewModel? deletedCredentialVM = null;
            void OnDeleteCredentialRequested(object? sender, CredentialViewModel vm) { deletedCredentialVM = vm; }
            Services.Messenger.DeleteCredential.Subscribe += OnDeleteCredentialRequested;
            credentialVM.Delete();
            Services.Messenger.DeleteCredential.Subscribe -= OnDeleteCredentialRequested;

            Assert.AreEqual(credentialVM, deletedCredentialVM);
        }

        private void ConfigureServices()
        {
            ServicesMock
                .Override<IClock>(new MockClock
                {
                    UniversalTime = DateTimeOffset.UnixEpoch.AddMilliseconds(Timestamp1)
                })
                .Override<ITotpGenerator>(new MockTotpGenerator(new Dictionary<string, string>
                {
                    { Secret1, Totp1 },
                    { Secret2, Totp2 }
                }))
                .Override<ILocalCredentialStorage>(new MockCredentialStorage(null));
        }
    }
}
