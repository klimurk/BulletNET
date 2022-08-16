namespace BulletNET.Services.UserDialogService.Interfaces
{
    public interface IOpenFileDialogService
    {
        Task<string> SelectFileGetPath();
    }
}