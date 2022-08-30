using BulletNET.Database.Entities.Base.Interfaces;
using BulletNET.Services.UserDialogService.Interfaces;

namespace BulletNET.Services.UserDialogService
{
    internal class UserDialog : IUserDialogService
    {
        #region Edit

        public bool Edit(object item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return item switch
            {
                object => EditItem(item),
                _ => throw new ArgumentException("Not supported type"),
            };
        }

        private static bool EditItem(object item) => default;

        #endregion Edit

        #region Dialog

        public bool ConfirmInformation(IDBTranslateble Information, IDBTranslateble Caption)
        {
            switch (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    return MessageBox.Show(
                        Information.DescriptionEn, Caption.DescriptionEn,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information) == MessageBoxResult.Yes;

                case "de":

                    return MessageBox.Show(
                        Information.DescriptionDe, Caption.DescriptionDe,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information) == MessageBoxResult.Yes;

                default:

                    return MessageBox.Show(
                        Information.DescriptionLocal, Caption.DescriptionLocal,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information) == MessageBoxResult.Yes;
            }
        }

        public bool ConfirmInformation(string Information, string Caption) => MessageBox
            .Show(
                Information, Caption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Information)
                == MessageBoxResult.Yes;

        public bool ConfirmWarning(IDBTranslateble Warning, IDBTranslateble Caption)
        {
            switch (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    return MessageBox.Show(
                        Warning.DescriptionEn, Caption.DescriptionEn,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes;

                case "de":

                    return MessageBox.Show(
                        Warning.DescriptionDe, Caption.DescriptionDe,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes;

                default:

                    return MessageBox.Show(
                        Warning.DescriptionLocal, Caption.DescriptionLocal,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes;
            }
        }

        public bool ConfirmWarning(string Warning, string Caption) => MessageBox
            .Show(
                Warning, Caption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning)
                == MessageBoxResult.Yes;

        public bool ConfirmError(IDBTranslateble Error, IDBTranslateble Caption)
        {
            switch (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    return MessageBox.Show(
                        Error.DescriptionEn, Caption.DescriptionEn,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error) == MessageBoxResult.Yes;

                case "de":

                    return MessageBox.Show(
                        Error.DescriptionDe, Caption.DescriptionDe,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error) == MessageBoxResult.Yes;

                default:

                    return MessageBox.Show(
                        Error.DescriptionLocal, Caption.DescriptionLocal,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error) == MessageBoxResult.Yes;
            }
        }

        public bool ConfirmError(string Error, string Caption) => MessageBox
            .Show(
                Error, Caption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Error)
                == MessageBoxResult.Yes;

        #endregion Dialog

        #region Show

        public void ShowInformation(IDBTranslateble Information, IDBTranslateble Caption)
        {
            switch (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    MessageBox.Show(
                        Information.DescriptionEn, Caption.DescriptionEn,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;

                case "de":

                    MessageBox.Show(
                        Information.DescriptionDe, Caption.DescriptionDe,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;

                default:

                    MessageBox.Show(
                        Information.DescriptionLocal, Caption.DescriptionLocal,
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;
            }
        }

        public void ShowInformation(string Information, string Caption) =>
            MessageBox.Show(
                Information, Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Information);

        public void ShowWarning(IDBTranslateble Warning, IDBTranslateble Caption)
        {
            switch (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    MessageBox.Show(
                        Warning.DescriptionEn, Caption.DescriptionEn,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    break;

                case "de":

                    MessageBox.Show(
                        Warning.DescriptionDe, Caption.DescriptionDe,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    break;

                default:

                    MessageBox.Show(
                        Warning.DescriptionLocal, Caption.DescriptionLocal,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    break;
            }
        }

        public void ShowWarning(string Warning, string Caption) =>
            MessageBox.Show(
                Warning, Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

        public void ShowError(IDBTranslateble Error, IDBTranslateble Caption)
        {
            switch (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName)
            {
                case "en":
                    MessageBox.Show(
                        Error.DescriptionEn, Caption.DescriptionEn,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;

                case "de":

                    MessageBox.Show(
                        Error.DescriptionDe, Caption.DescriptionDe,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;

                default:

                    MessageBox.Show(
                        Error.DescriptionLocal, Caption.DescriptionLocal,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;
            }
        }

        public void ShowError(string Error, string Caption)
        {
            MessageBox.Show(
                Error, Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        #endregion Show
    }
}