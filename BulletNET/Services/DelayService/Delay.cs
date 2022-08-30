using BulletNET.Services.Devices.Base;
using BulletNET.Services.UserDialogService.Interfaces;

namespace BulletNET.Services.DelayService;

public class Delay : Test, IDelay
{
    private readonly IUserDialogService _IUserDialogService;

    public Delay(IUserDialogService IUserDialogService)
    {
        _IUserDialogService = IUserDialogService;
    }

    public bool MakeDelay(int millis)
    {
        StartTest("Delay " + millis.ToString());
        IsScheduled = true;

        Thread.Sleep(millis);
        IsPassed = true;
        EndTest();
        if (!IsPassed && _IUserDialogService.ConfirmInformation("> " + TestName + " < failed. Retry?", "Test failed"))
            MakeDelay(millis);

        return IsPassed;
    }
}