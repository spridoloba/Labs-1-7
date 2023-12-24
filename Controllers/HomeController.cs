using DSP.Models;
using DSP.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI;
using System.Diagnostics;


namespace DSP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;
        private readonly FileTransferService _fileTransferService;
        private readonly DatabaseService _databaseService;
        private readonly ILogger<HomeController> _logger;

        
        public HomeController(ApiService apiService, FileTransferService fileTransferService, DatabaseService databaseService, ILogger<HomeController> logger)
        {
            _apiService = apiService;
            _logger = logger;
            _databaseService = databaseService;
            _fileTransferService = fileTransferService;
        }

        public IActionResult Index()
        {
            return View();
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

        [HttpPost]
        public async Task<IActionResult> FetchData(OfferFormData formData)
        {
            try
            {



                var imageLinks = await _apiService.FetchImageLinksForParticipant(formData.TeaserId, formData.Participants);
                var result = await _apiService.FetchDataForParticipant(formData.TeaserId, formData.Participants);
                var apiResponse = await _apiService.FetchDataForParticipant(formData.TeaserId, formData.Participants);
               
                var yazik = await _apiService.FetchLanguageForParticipant(formData.TeaserId, formData.Participants);

               
                var (geo, buttonText) = await _apiService.GetLanguageDetails(yazik, "readmore.txt");


               





                string uploadedImagePath = null;
                foreach (var imageUrl in imageLinks)
                {
                    string sftpServer = "163.";
                    string username = "img.d";
                    string password = "YHefq";

                    _logger.LogInformation("Calling DownloadAndUploadImageAsync");

                   
                    string remoteDirectory = imageUrl.EndsWith(".mp4") ? "267/" : "266/";
                    uploadedImagePath = await _fileTransferService.DownloadAndUploadImageAsync(imageUrl, sftpServer, username, password);
                    _logger.LogInformation($"Image path after upload: {uploadedImagePath}");
                }

                if (!string.IsNullOrEmpty(uploadedImagePath))
                {
                    
                    var offerRecord = new OfferRecord
                    {

                       
                        Country = formData.GEO,
                        RoiForSources = formData.ROISource,
                        RoiForUsers = formData.ROIUsers,
                        Cat = formData.Category,
                        Lang = geo,
                        LangFilter = geo,
                        AdType = formData.TeaserCategory,
                       
                        Offer = $"{formData.Offer}{new Random().Next(1000000, 9999999)}_{formData.GEO}",
                        Payout = formData.Payout,
                        ButtonText = buttonText,
                        Owner = formData.Participants,
                        MgidId = Convert.ToInt32(formData.TeaserId),
                        Link = apiResponse.url,  
                        Title = apiResponse.title, 
                        Image = uploadedImagePath,
                        Tags = formData.TeaserTags,
                        
                    };


                    await offerRecord.SetCategoryAsync(apiResponse.Category.Id, _databaseService);
                    long newRecordId = await _databaseService.InsertOfferRecordAsync(offerRecord);
                    ViewBag.StatusMessage = $"Запис з айді {newRecordId} успішно створенно.";
                }

                return View("Index", formData); 
            }
            catch (Exception ex)
            {
               
                ViewBag.ErrorMessage = $"Помилка: {ex.Message}";
                return View("Index", formData);
            }
            
            


        }


        



        public IActionResult SecondPage()
        {
            return View();
        }
    }
}
