using DSP.Models;
using DSP.Service;
using Microsoft.AspNetCore.Mvc;

namespace DSP.Controllers
{
    public class OperationController : Controller
    {
        private readonly DatabaseService _databaseService;
        private readonly ParticipantAuthService _participantAuthService;
        public OperationController(DatabaseService databaseService, ParticipantAuthService participantAuthService)
        {
            _databaseService = databaseService;
            _participantAuthService = participantAuthService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PerformOperation(OperationViewModel formData)
        {
           
            if (!await _participantAuthService.AuthenticateParticipantAsync(formData.Login, formData.Password))
            {
                ViewBag.ErrorMessage = "Не вірний логін або пароль.";
                return View("~/Views/Home/SecondPage.cshtml", formData);
            }

           
            if (!await _databaseService.IsTeaserOwnedByParticipant(formData.TeaserId, formData.Login))
            {
                ViewBag.ErrorMessage = "Ви не володієте цим тизером, або його не існує.";
                return View("~/Views/Home/SecondPage.cshtml", formData);
            }


            if (formData.Operation == "enable")
            {
                await _databaseService.InsertTaskEnableAsync(formData.TeaserId);
                ViewBag.StatusMessage = "Операція 'Включити' виконана успешно.";
                return View("~/Views/Home/SecondPage.cshtml", formData);
            }

            if (formData.Operation == "changeRoi")
            {
                await _databaseService.InsertTaskRoiAsync(formData.TeaserId, formData.RoiSource, formData.RoiUsers);
                ViewBag.StatusMessage = "Операція 'Змінити Рої' виконана успішно.";
                return View("~/Views/Home/SecondPage.cshtml", formData);
            }

            if (formData.Operation == "changeMode")
            {
                await _databaseService.InsertTaskModeAsync(formData.TeaserId, formData.Mode);
                ViewBag.StatusMessage = "Операція 'Змінити режим' виконана успішно.";
                return View("~/Views/Home/SecondPage.cshtml", formData);
            }

            if (formData.Operation == "changePayout")
            {
                await _databaseService.InsertTaskPayoutAsync(formData.TeaserId, formData.Payout);
                ViewBag.StatusMessage = "Операція 'Змінити виплату' виконана успішно.";
                return View("~/Views/Home/SecondPage.cshtml", formData);
            }

            if (formData.Operation == "changeClimit")
            {
                await _databaseService.InsertTaskClimitAsync(formData.TeaserId, formData.Climit);
                ViewBag.StatusMessage = "Операція 'Змінити C_Limit' виконана успішно.";
                return View("~/Views/Home/SecondPage.cshtml", formData);
            }

            if (formData.Operation == "disable")
            {
                await _databaseService.InsertTaskDisableAsync(formData.TeaserId);
                ViewBag.StatusMessage = "Операція 'Виключити' виконана успішно.";
                return View("~/Views/Home/SecondPage.cshtml", formData);
            }
           
            

            ViewBag.StatusMessage = "Я навіть не знаю, як ти зробив цю помилку, напиши в тг.";
            return View("~/Views/Home/SecondPage.cshtml", formData); 
        }


    }
}
