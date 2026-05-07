using System.Collections.Generic;
using System.Threading.Tasks;

namespace Extractor.Gui.Services.Interfaces;

public interface IFilePicker
{
    Task<IEnumerable<string>> PickMultipleItemsAsync(string title);
    Task<string?> PickSingleItemAsync(string title);
}