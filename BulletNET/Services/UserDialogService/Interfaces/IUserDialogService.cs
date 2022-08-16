using BulletNET.Database.Entities.Base.Interfaces;

namespace Pallet.Services.UserDialogService.Interfaces
{
    public interface IUserDialogService
    {
        bool Edit(object item);

        public bool ConfirmWarning(string Warning, string Caption);

        public bool ConfirmWarning(IDBTranslateble Error, IDBTranslateble Caption);

        public bool ConfirmError(string Error, string Caption);

        public bool ConfirmError(IDBTranslateble Error, IDBTranslateble Caption);

        public bool ConfirmInformation(string Information, string Caption);

        public bool ConfirmInformation(IDBTranslateble Error, IDBTranslateble Caption);

        public void ShowInformation(string Information, string Caption);

        public void ShowInformation(IDBTranslateble Error, IDBTranslateble Caption);

        public void ShowWarning(string Warning, string Caption);

        public void ShowWarning(IDBTranslateble Error, IDBTranslateble Caption);

        public void ShowError(string Error, string Caption);

        public void ShowError(IDBTranslateble Error, IDBTranslateble Caption);
    }
}