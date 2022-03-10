using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMEKSA.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class HrController : ControllerBase
    {
        private readonly IHrRep rep;

        public HrController(IHrRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllMedicalRep()
        {
            return Ok(rep.GetAllMedicalRep());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllSalesRep()
        {
            return Ok(rep.GetAllSalesRep());
        }


        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetActualWorkingDaysSales(string userid)
        {
            return Ok(rep.GetActualWorkingDaysSales(userid));
        }

        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetActualWorkingDaysMedical(string userid)
        {
            return Ok(rep.GetActualWorkingDaysMedical(userid));
        }

     
        [Route("[controller]/[Action]/{userid}")]
        [HttpGet]
        public IActionResult GetTimeOffDays(string userid)
        {
            return Ok(rep.GetTimeOffDays(userid));
        }
    }
}
