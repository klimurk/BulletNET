namespace BulletNET.Services.BarcodeReader.Interface
{
    public interface IBarcodeCRC
    {
        long DeleteCRC(long x);

        bool InRange(long x);

        bool IsAllCRCOk(long x);

        long MakeAllCRC(int number, int trida);
    }
}