using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Blob;
using WebBlob_Demo.Models;

namespace WebBlob_Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly List<Image> imagesList;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            imagesList = new List<Image>();
        }

        public async Task<IActionResult> Index(string name)
        {
            var storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("StorageConnection"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("filepublic");
            BlobContinuationToken? blobContinuationToken = null;
            do
            {
                var results = await container.ListBlobsSegmentedAsync(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (var item in results.Results)
                {
                    Console.WriteLine(item.Uri);
                    imagesList.Add(new Image(item.Uri.Segments[^1], item.Uri.AbsoluteUri));
                }
            } while (blobContinuationToken != null);

            return View(imagesList);
        }
        

        [HttpPost]
        public async Task<ActionResult> UploadFile()
        {
            var file = Request.Form.Files[0];
            if (file.Length <= 0) return RedirectToAction("Index");
            var storageAccount = CloudStorageAccount.Parse(_configuration.GetConnectionString("StorageConnection"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("filepublic");
            var blockBlob = container.GetBlockBlobReference(file.FileName);
            //blockBlob.UploadFromStream(file.InputStream);
            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}