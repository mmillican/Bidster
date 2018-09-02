using System.Collections.Generic;
using Bidster.Helpers;
using Bidster.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bidster.Controllers
{
    public abstract class BaseController : Controller
    {
        private const string NoticeDataKey = "_Notices";
        
        protected void AddInfoNotice(string message)
        {
            AddDisplayNotice("alert-info", message);
        }

        protected void AddSuccessNotice(string message)
        {
            AddDisplayNotice("alert-success", message);
        }

        protected void AddWarningNotice(string message)
        {
            AddDisplayNotice("alert-warning", message);
        }

        protected void AddErrorNotice(string message)
        {
            AddDisplayNotice("alert-danger", message);
        }

        private void AddDisplayNotice(string type, string message)
        {
            var notices = TempData.GetTempData<List<NoticeModel>>(NoticeDataKey);
            if (notices == null)
                notices = new List<NoticeModel>();

            notices.Add(new NoticeModel(type, message));

            TempData.SaveTempData(NoticeDataKey, notices);
        }
    }
}