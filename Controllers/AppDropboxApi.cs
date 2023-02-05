using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using sandboxEr.Repositories.Interfaces;

namespace kkl.Services.Controllers
{ 

    [ApiController]

    public class AppDropboxApiController : ControllerBase
    {
        private readonly IGetToken _getToekn;

        public AppDropboxApiController(IGetToken getToekn)
        {
            _getToekn = getToekn;
        }

        [HttpPost]
        [Route("generateToken")]
        [Consumes("multipart/form-data","application/json", "application/xml")]
        public virtual async Task < IActionResult > Token([FromBody]TokenParam? token) { 
         try
            {
                  return Ok(await _getToekn.GenerateToken(token));
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
}   
        [HttpGet]
        [Route("generateCode")]
        [Consumes("multipart/form-data","application/json", "application/xml")]
        public virtual async Task <IActionResult> Code() { 
         try
            {
               
                  var code=await _getToekn.GenerateCode();
                  return Ok(code);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
       
            
            
}   
       
  
  
       



   
      


        }
