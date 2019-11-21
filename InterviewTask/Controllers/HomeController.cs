using System.Web.Mvc;
//### start
using InterviewTask.Models;
using InterviewTask.Services;
using System.Collections.Generic;
using System;
using System.Net;
//### finish

namespace InterviewTask.Controllers
{
    public class HomeController : Controller
    {

        private HelperServiceRepository oHSR; 

        public HomeController()
        {
            oHSR = new HelperServiceRepository();
        } // end of constructor
        
        public ActionResult Index()
        {
            var listHSM = oHSR.Get();
            foreach(var item in listHSM)
            {
                OpenClosedStatus(item); // add hours
            }
            return View(listHSM);
        }

        // GET: Home/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            HelperServiceModel helperServiceModel = oHSR.Get(id.Value);
            if (helperServiceModel == null)
            {
                return HttpNotFound();
            }

            OpenClosedStatus(helperServiceModel); // add hours
            return View(helperServiceModel);
        }

        /// <summary>
        /// Open-Closed helper function
        /// </summary>
        /// <param name="helperServiceModel"></param>
        /// <returns>Open/Closed text, and style-class</returns>
        //public static string OpenClosedStatus(HelperServiceModel helperServiceModel)
        public void OpenClosedStatus(HelperServiceModel helperServiceModel)
        {
            string sRet = "We're sorry, we are temporarily unable to display Opening Times";
            var dayOfWeek = DateTime.Today.DayOfWeek;
            var timeOfDay = DateTime.Now.TimeOfDay;
            int nOpen = 0;
            int nClose = 0;
            bool bOpenNClosed = false;
            int nOffset = 0; 
            string sOpenDay = "";
            const string sorry1 = "Closed: We're sorry, we are temporarily unable to display the next Opening Time."; // bad times
            const string sorry2 = "We're sorry, we are temporarily unable to display Opening Times";
            const string sorry3 = "We're sorry, we are temporarily unable to display Opening Times"; // bad day default
            const string sorry4 = "Closed: please see one of our other Helper Services.";
            string sHoursStyle = "bg-color-light-grey";
            try
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (helperServiceModel.SundayOpeningHours == null) throw new ApplicationException(sorry2);
                        nOpen = helperServiceModel.SundayOpeningHours[0];
                        nClose = helperServiceModel.SundayOpeningHours[1];
                        break;
                    case DayOfWeek.Monday:
                        if (helperServiceModel.MondayOpeningHours == null) throw new ApplicationException(sorry2);
                        nOpen = helperServiceModel.MondayOpeningHours[0];
                        nClose = helperServiceModel.MondayOpeningHours[1];
                        break;
                    case DayOfWeek.Tuesday:
                        if (helperServiceModel.TuesdayOpeningHours == null) throw new ApplicationException(sorry2);
                        nOpen = helperServiceModel.TuesdayOpeningHours[0];
                        nClose = helperServiceModel.TuesdayOpeningHours[1];
                        break;
                    case DayOfWeek.Wednesday:
                        if (helperServiceModel.WednesdayOpeningHours == null) throw new ApplicationException(sorry2);
                        nOpen = helperServiceModel.WednesdayOpeningHours[0];
                        nClose = helperServiceModel.WednesdayOpeningHours[1];
                        break;
                    case DayOfWeek.Thursday:
                        if (helperServiceModel.ThursdayOpeningHours == null) throw new ApplicationException(sorry2);
                        nOpen = helperServiceModel.ThursdayOpeningHours[0];
                        nClose = helperServiceModel.ThursdayOpeningHours[1];
                        break;
                    case DayOfWeek.Friday:
                        if (helperServiceModel.FridayOpeningHours == null) throw new ApplicationException(sorry2);
                        nOpen = helperServiceModel.FridayOpeningHours[0];
                        nClose = helperServiceModel.FridayOpeningHours[1];
                        break;
                    case DayOfWeek.Saturday:
                        if (helperServiceModel.SaturdayOpeningHours == null) throw new ApplicationException(sorry2);
                        nOpen = helperServiceModel.SaturdayOpeningHours[0];
                        nClose = helperServiceModel.SaturdayOpeningHours[1];
                        break;
                    default: // null
                        throw new ApplicationException(sorry3);                        
                }
                // check if open now, assume nOpen=0 for closed all day
                if (nOpen > 0)
                {
                    if (TimeSpan.Compare(timeOfDay, TimeSpan.FromHours(nOpen)) > 0)
                    {
                        if (TimeSpan.Compare(timeOfDay, TimeSpan.FromHours(nClose)) < 0) 
                        { // we are open
                            bOpenNClosed = true;
                        }
                    }
                }

                if (bOpenNClosed)
                { // we are open!
                    sRet = "Open today until " + nClose.ToString() + ((nClose > 12) ? "PM" : "AM");
                    sHoursStyle = "bg-color-donation-orange";
                }
                else
                { // closed, find next open day/time
                    nOpen = 0;                    
                    for(int day = 0; day < 7; day++)
                    {
                        if (nOpen > 0) break;
                        switch (dayOfWeek)
                        {
                            case DayOfWeek.Sunday: nOffset = 0; break;
                            case DayOfWeek.Monday: nOffset = 1; break;
                            case DayOfWeek.Tuesday: nOffset = 2; break;
                            case DayOfWeek.Wednesday: nOffset = 3; break;
                            case DayOfWeek.Thursday: nOffset = 4; break;
                            case DayOfWeek.Friday: nOffset = 5; break;
                            case DayOfWeek.Saturday: nOffset = 6; break;
                        }
                        switch (day+nOffset)
                        {
                            case 0:
                            case 7:
                                if (helperServiceModel.MondayOpeningHours == null) throw new ApplicationException(sorry1);
                                nOpen = helperServiceModel.MondayOpeningHours[0];
                                sOpenDay = "Monday";
                                break;
                            case 1:
                            case 8:
                                if (helperServiceModel.TuesdayOpeningHours == null) throw new ApplicationException(sorry1);
                                nOpen = helperServiceModel.TuesdayOpeningHours[0];
                                sOpenDay = "Tuesday";
                                break;
                            case 2:
                            case 9:
                                if (helperServiceModel.WednesdayOpeningHours == null) throw new ApplicationException(sorry1);
                                nOpen = helperServiceModel.WednesdayOpeningHours[0];
                                sOpenDay = "Wednesday";
                                break;
                            case 3:
                            case 10:
                                if (helperServiceModel.ThursdayOpeningHours == null) throw new ApplicationException(sorry1);
                                nOpen = helperServiceModel.ThursdayOpeningHours[0];
                                sOpenDay = "Thursday";
                                break;
                            case 4:
                            case 11:
                                if (helperServiceModel.FridayOpeningHours == null) throw new ApplicationException(sorry1);
                                nOpen = helperServiceModel.FridayOpeningHours[0];
                                sOpenDay = "Friday";
                                break;
                            case 5: 
                            case 12:
                                if (helperServiceModel.SaturdayOpeningHours == null) throw new ApplicationException(sorry1);
                                nOpen = helperServiceModel.SaturdayOpeningHours[0];
                                sOpenDay = "Saturday";
                                break;
                            case 6:
                            case 13:
                                if (helperServiceModel.SundayOpeningHours == null) throw new ApplicationException(sorry1);
                                nOpen = helperServiceModel.SundayOpeningHours[0];
                                sOpenDay = "Sunday";
                                break;
                        }
                        // check for opening hour
                        if (nOpen == 0) throw new ApplicationException(sorry4);
                        sRet = "Closed: Re-opens on " + sOpenDay + " at " + nOpen.ToString() + ((nOpen > 12) ? "PM" : "AM");
                    }
                }
            }
            catch (Exception ex)
            {
                sRet = ex.Message;                
            }

            //return sRet;
            helperServiceModel.HoursText = sRet;
            helperServiceModel.HoursStyle = sHoursStyle;
        } // end of (o)s


    }
}