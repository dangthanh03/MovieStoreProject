namespace MovieStoreMvc.Repositories.Abstract
{
    public interface IFileService
    {
        public string UploadImage(IFormFile imageFile);
        public string UploadVideo(IFormFile file);


    }
}
