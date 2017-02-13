﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Master.Contract;
using PickC.Services;
using PickC.Services.DTO;
using PickC.Internal.ViewModals;
using System.IO;

namespace PickC.Internal.Areas.Internal.Controllers
{
    [WebAuthFilter]
    [PickCEx]
    public class DriverController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult> Driver()
        {
            var driverList = await new DriverService(AUTHTOKEN, p_mobileNo).DriversListAsync();
            return View(driverList);
        }

        [HttpGet]
        public async Task<ActionResult> Add()
        {
            var driverVm = new DriverVm
            {
                driverLookupDTO = await new DriverService(AUTHTOKEN, p_mobileNo).LookUpDataAsync(),
                driver = new Driver()
            };

            return View(driverVm);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string driverID)
        {
            Task<Driver> taskDriver = new DriverService(AUTHTOKEN, p_mobileNo).DriverInfoAsync(driverID);
            Task<DriverLookupDTO> taskDriverLookupDTO = new DriverService(AUTHTOKEN, p_mobileNo).LookUpDataAsync();

            await Task.WhenAll(taskDriver, taskDriverLookupDTO);

            var driverVm = new DriverVm
            {
                driverLookupDTO = await taskDriverLookupDTO,
                driver = await taskDriver
            };

            return View(driverVm);
        }

        [HttpPost]
        public async Task<ActionResult> SaveDriver(Driver driver)
        {
            var result = await new DriverService(AUTHTOKEN, p_mobileNo).SaveDriverAsync(driver);
            return RedirectToAction("Driver", "Driver");
        }

        public async Task<JsonResult> GetAttachmentData()
        {
            var driverLookupDTO = await new DriverService(AUTHTOKEN, p_mobileNo).LookUpDataAsync();
            return Json(driverLookupDTO, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> AddAttachment(DriverAttachments attachment)
        {
            var driverLookupDTO = await new DriverService(AUTHTOKEN, p_mobileNo).LookUpDataAsync();
            try
            {
                string mapPath = Server.MapPath("~/Attachments");
                if (!Directory.Exists(mapPath))
                {
                    Directory.CreateDirectory(mapPath);
                }
                attachment.imagePath.SaveAs(mapPath + Path.GetFileName(attachment.imagePath.FileName));
            }
            catch (Exception ex)
            {
                throw;
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}