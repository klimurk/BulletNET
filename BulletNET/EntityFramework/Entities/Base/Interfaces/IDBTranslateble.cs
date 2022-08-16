namespace BulletNET.Database.Entities.Base.Interfaces
{
    public interface IDBTranslateble
    {
        string DescriptionEn { get; set; }
        string DescriptionDe { get; set; }
        string DescriptionLocal { get; set; }
    }
}