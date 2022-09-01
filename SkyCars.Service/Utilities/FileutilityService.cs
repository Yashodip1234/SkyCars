using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SkyCars.Core;
using SkyCars.Core.DomainEntity.Grid;
using SkyCars.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace SkyCars.Services.Utilities
{

    public partial class FileutilityService : IFileutilityService
    {
        #region Fields

        #endregion

        #region Ctor

        public FileutilityService()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Create zip file from file path list
        /// </summary>
        /// <param name="filePathList">List of full physical file paths</param>
        /// <returns>zip file</returns>
        public virtual async Task<MemoryStream> GetZipFile(IList<string> filePathList)
        {
            using var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create))
            {
                foreach (var curFilePath in filePathList)
                {
                    var botFileName = Path.GetFileName(curFilePath);
                    var entry = archive.CreateEntry(botFileName);
                    using var entryStream = entry.Open();
                    using var fileStream = File.OpenRead(curFilePath);
                    await fileStream.CopyToAsync(entryStream);
                }
            }
            return ms;
        }


        #endregion

    }
}