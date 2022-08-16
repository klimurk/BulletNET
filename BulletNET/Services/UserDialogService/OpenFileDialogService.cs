using BulletNET.Services.UserDialogService.Interfaces;
using Microsoft.Win32;

namespace BulletNET.Services.UserDialogService;

public class OpenFileDialogService : IOpenFileDialogService
{
    public async Task<string> SelectFileGetPath()
    {
        // open file
        //OpenFileDialogArguments dialogArgs = new()
        //{
        //    Width = 1500,
        //    Height = 800,
        //    Filters = "Text files|*.txt|All files|*.*",
        //};
        //string dialogHostName = (App.Services.GetService(typeof(MainWindowViewModel)) as MainWindowViewModel)?.DialogHostName;

        //OpenFileDialogResult result = await OpenFileDialog.ShowDialogAsync("dialogHost", dialogArgs);

        //if (!result.Canceled)
        //{
        //    return string.IsNullOrEmpty(result.FileInfo.FullName) ? string.Empty : result.FileInfo.FullName;
        //}
        //else
        //{
        //    return string.Empty;
        //}
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FilterIndex = 1,
            Multiselect = false,
            Title = "Select sequence file",
        };
        openFileDialog.ShowDialog();

        return string.IsNullOrEmpty(openFileDialog.FileName) ? string.Empty : openFileDialog.FileName;
    }
}