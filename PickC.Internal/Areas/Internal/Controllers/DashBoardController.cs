﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using PickC.Services;
using PickC.Internal.ViewModals;
using PickC.Services.DTO;
using Operation.Contract;

namespace PickC.Internal.Areas.Internal.Controllers
{
    [WebAuthFilter]
    [PickCEx]
    public class DashboardController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return View(await GetTripMonitorData());
        }

        [HttpGet]
        public async Task<JsonResult> GetTripMonitorInfo()
        {
            return Json(await GetTripMonitorData(), JsonRequestBehavior.AllowGet);
        }

        public async Task<List<TripMonitorVm>> GetTripMonitorData()
        {
            var tripMonitorList = await new TripMonitorService(AUTHTOKEN, p_mobileNo)
                                            .TripMonitorListAsync();

            var tripMonitorData = new List<TripMonitorVm>();
            if (tripMonitorList != null)
            {
                for (var i = 0; i < tripMonitorList.Count; i++)
                {
                    var tripMonitor = new TripMonitorVm();
                    tripMonitor.address = new Address
                    {
                        address = "",
                        lat = tripMonitorList[i].Latitude,
                        lng = tripMonitorList[i].Longitude,
                    };
                    tripMonitor.title = tripMonitorList[i].DriverID + " - " + tripMonitorList[i].TripID;

                    tripMonitorData.Add(tripMonitor);
                }
            }


            return tripMonitorData;
        }

        public async Task<ActionResult> GetDriversList()
        {
            var driverList = await new DriverService(AUTHTOKEN, p_mobileNo).DriversListAsync();
            var tripMonitor = await GetTripMonitorData();

            var driverMonitorVm = new DriverMonitorVm()
            {
                driverList = driverList,
                tripMonitorVmList = tripMonitor
            };

            return View(driverMonitorVm);
        }

        [HttpGet]
        public async Task<ActionResult> CurrentBookings(BookingDTO search)
        {
            var currentbookings = await new SearchService(AUTHTOKEN, p_mobileNo).SearchCurrentBookingAsync(search);
            var bookingSearchVM = new BookingSearchVM();
            bookingSearchVM.booking =currentbookings;

            return View("CurrentBookings", bookingSearchVM);
        }

        [HttpPost]
        public async Task<ActionResult> CurrentBookings(BookingSearchVM booking)
        {
            var currentbooking = await new SearchService(AUTHTOKEN, p_mobileNo).SearchBookingByDateAsync(booking.dates.fromDate, booking.dates.toDate);
            var BookingSearchVM = new BookingSearchVM();
            return View("CurrentBookings", currentbooking);

        }

        public async Task<JsonResult> GetDriverByName(string driverName)
        {
            var driverlist = await new DriverService(AUTHTOKEN, p_mobileNo).GetDriverByName(driverName);
            return Json(driverlist, JsonRequestBehavior.AllowGet);
        }
    }
}