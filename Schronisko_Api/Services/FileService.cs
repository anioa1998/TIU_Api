using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Schronisko_Api.Services
{
    public class FileService
    {
        public bool DeleteSelectedFile(string pathToDelete)
        {
            try
            {
                if (File.Exists(pathToDelete))
                {
                    File.Delete(pathToDelete);
                    return true;
                }
                else return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var pathToSave = Path.Combine(@"D:\Semestr VI\TIU\Zad1\Zad1App\src", "assets"); //sztywne ustawienie do folderu assets

            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = @"/assets/" + fileName;//Path.Combine("assets", fileName);
                dbPath = dbPath.Replace(" ", "");

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return dbPath;
            }
            else return "";
        }
    }
}
