using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ITvitaeChat2.Backend.Controllers
{
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        // Potentially dangerous file extensions
        private readonly string[] NotAllowedFiles = new string[] { "exe", "pif", "application", "gadget", "msi", "msp", "com", "scr", "hta", "cpl", "msc", "jar", "bat", "cmd", "vb", "vbs", "js",
        "jse", "ws", "wsf", "wsc", "wsh", "ps1", "ps1xml", "ps2", "ps2xml", "psc1", "psc2", "msh", "msh1", "msh2", "mshxml", "msh1xml", "msh2xml", "scf", "lnk", "inf", "reg", "doc", "xls",
        "ppt", "docm", "dotm", "xlsm", "xltm", "xlam", "pptm", "potm", "ppam", "ppsm", "sldm" };
        
        // The server envirement
        private static IWebHostEnvironment Environment;
        
        public FileController(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        /// <summary>
        /// Saves a file to the specified folder. Be sure to use an unique foldername for each user.
        /// </summary>
        /// <param name="folderName">Folder location to save to.</param>
        /// <param name="formFile">Microsoft.AspNetCore.Http.IFormFile wich contains the user choosen file.</param>
        /// <returns>The full file location</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Post(string folderName, IFormFile formFile)
        {
            // Check if there is anything inside the parameter formFile
            if (formFile.Length == 0) return BadRequest("No file was attached to the post");

            // Get the file extension
            string extension = Path.GetExtension(formFile.FileName).ToLower();

            // Check if the file is allowed
            for (int i = 0; i < NotAllowedFiles.Length; i++)
            {
                if (extension.Equals($".{NotAllowedFiles[i]}")) return BadRequest($"The file extension '.{NotAllowedFiles[i]}' is not allowed");
            }

            try
            {
                // Create new directory if it doesn't exist yet
                if (!Directory.Exists(Environment.ContentRootPath + $"\\{folderName}\\"))
                {
                    Directory.CreateDirectory(Environment.ContentRootPath + $"\\{folderName}\\");
                }

                // Save the file to the specified folder
                using (FileStream filestream = System.IO.File.Create($"{Environment.ContentRootPath}\\{folderName}\\{formFile.FileName}"))
                {
                    formFile.CopyTo(filestream);
                    filestream.Flush();
                    //CreatedAtActionResult caa = new CreatedAtActionResult("Test", "File", "api/files", $"\\{folderName}\\{formFile.FileName}");

                    return CreatedAtAction(nameof(Post), $"\\{folderName}\\{formFile.FileName}");
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            
        }
    }
}
