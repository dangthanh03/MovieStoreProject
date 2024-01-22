using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class FileService : IFileService
    {
        private readonly Cloudinary _cloudinary;

        private readonly IWebHostEnvironment environment;

        public FileService(IWebHostEnvironment env, Cloudinary cloudinary)
        {
            this.environment = env;
            this._cloudinary = cloudinary;
        }
        [HttpPost]
        public string UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                         
                    };

                    var uploadResult = _cloudinary.Upload(uploadParams);

                  

                    return uploadResult.Url.ToString();
                }
                }
            

            return null;
        }


        public string UploadVideo(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new VideoUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                    };

                    var uploadResult = _cloudinary.Upload(uploadParams);


                    return uploadResult.Url.ToString();
                }
            }

            return null;
        }

    }
}
