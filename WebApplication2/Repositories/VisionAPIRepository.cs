using Google.Cloud.Vision.V1;   

namespace WebApplication2.Repositories
{
    public class VisionAPIRepository
    {
        private BucketsRepository _bucketsRepository;
        private string bucketName;
        public VisionAPIRepository(BucketsRepository bucketsRepository, IConfiguration config)
        {    
            bucketName = config["PosterBucket"];  
            _bucketsRepository = bucketsRepository;
        }

        public async Task<string> ProcessImage(string imageName)
        {
            var client = ImageAnnotatorClient.Create();

            //download the image from the bucket and save it a temp location

            string imageFilePath = await _bucketsRepository.DownloadImage(bucketName, imageName);
            
            var image = Image.FromFile(imageFilePath);
            var response = client.DetectText(image);
            var labels = response.Select(label => label.Description);
            string result = string.Join(", ", labels);

            return result;
        }
    }
}
