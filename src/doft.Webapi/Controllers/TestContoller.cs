using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("testSingleDataSuccess")]
        public IActionResult TestSingleDataSuccess()
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return Ok(ApiResponse<string>.Success(200, "Test", time));
        }

        [HttpGet]
        [Route("testSingleDataFail")]
        public IActionResult TestSingleDataFail()
        {
            var errorMessage = "Test error message";
            return Ok(ApiResponse<string>.Error(400, "Test", errorMessage));
        }

        [HttpGet]
        [Route("testListDataSuccess")]
        public IActionResult TestListDataSuccess()
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var data = new List<string> { time, time, time };
            return Ok(ApiResponse<List<string>>.Success(200, "Test", data));
        }

        [HttpPost]
        [Route("testDataSuccess")]
        public IActionResult TestDataSuccess([FromBody] TestData testData)
        {
            if (testData == null || string.IsNullOrEmpty(testData.Data))
            {
                return BadRequest(ApiResponse<string>.Error(400, "Test", "Invalid data"));
            }

            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return Ok(ApiResponse<string>.Success(200, "Test", time));
        }

    }

    public class TestData
    {
        public required string Data { get; set; }
    }
}