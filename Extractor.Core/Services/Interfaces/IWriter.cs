namespace Extractor.Core.Services.Interfaces;

public interface IWriter<T> where T : class
{
    void SaveData(T data);
}