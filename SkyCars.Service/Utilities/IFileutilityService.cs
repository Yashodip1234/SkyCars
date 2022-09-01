using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SkyCars.Services.Utilities
{
    public interface IFileutilityService
    {
        Task<MemoryStream> GetZipFile(IList<string> filePathList);
    }
}