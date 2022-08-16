using BulletNET.EntityFramework.Entities.Radar;

namespace BulletNET.Services.SequenceReaderService.Interfaces
{
    public interface ISequenceReader
    {
        (List<TestGroup> testGroupQueue, string errorString) ReadSequenceFile(string filepath);
    }
}