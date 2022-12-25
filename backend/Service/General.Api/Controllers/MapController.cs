using General.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace General.Api.Controllers
{
    public class MapController : ApiController
    {
        private readonly ILogger<MapController> _logger;
        private readonly IConvertVietNameseService _convertVietNameseService;

        public MapController(ILogger<MapController> logger, IConvertVietNameseService convertVietNameseService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _convertVietNameseService = convertVietNameseService ?? throw new ArgumentNullException(nameof(convertVietNameseService));
        }

        // Get List
        [HttpGet("GetProvinces")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Array>> GetProvincesData()
        {
            var json = JObject.Parse(System.IO.File.ReadAllText("map.json"))["cities"]
                          .Select(n => new { 
                              code = n["code"], 
                              name = n["name"],
                              nameWithType = n["nameWithType"],
                              nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                              nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString())
                          }).ToArray();

            return json;
        }

        [HttpGet("GetDistricts")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Array>> GetDistrictsData()
        {
            var json = JObject.Parse(System.IO.File.ReadAllText("map.json"))["districts"]
                          .Select(n => new { 
                              code = n["code"], 
                              name = n["name"], 
                              nameWithType = n["nameWithType"],
                              nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                              nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                              parentCode = n["parentCode"]
                              //path = n["path"]
                          }).ToArray();

            return json;
        }

        [HttpGet("GetWards")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Array>> GetWardsData()
        {
            var json = JObject.Parse(System.IO.File.ReadAllText("map.json"))["wards"]
                          .Select(n => new {
                              code = n["code"],
                              name = n["name"],
                              nameWithType = n["nameWithType"],
                              nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                              nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                              parentCode = n["parentCode"]
                              //path = n["path"]
                          }).ToArray();

            return json;
        }

        // Filter
        [HttpPost("FilterDisctrict")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<Array>> FilterDisctrict(string provinceCode)
        {
            if (provinceCode == null) return BadRequest();

            var result = JObject.Parse(System.IO.File.ReadAllText("map.json"))["districts"]
                            .Where(x => x["parentCode"].Value<string>() == provinceCode)
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"],
                                nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                                nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                                parentCode = n["parentCode"]
                                //path = n["path"]
                            }).ToArray();

            return Ok(result);
        }

        [HttpPost("FilterWard")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<Array>> FilterWard(string districtCode)
        {
            if (districtCode == null) return BadRequest();

            var result = JObject.Parse(System.IO.File.ReadAllText("map.json"))["wards"]
                            .Where(x => x["parentCode"].Value<string>() == districtCode)
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"],
                                nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                                nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                                parentCode = n["parentCode"]
                                //path = n["path"]
                            }).ToArray();

            return Ok(result);
        }

        // Get Coordinates by Code
        [HttpPost("GetCoordinateProvinceByCode")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCoordinateProvinceByCode(string code)
        {
            if (code == null) return BadRequest();

            var result = JObject.Parse(System.IO.File.ReadAllText("map.json"))["cities"]
                            .Where(x => x["code"].Value<string>() == code)
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"],
                                nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                                nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                                coordinates = n["coordinates"]
                            }).FirstOrDefault();

            return Ok(result);
        }

        [HttpPost("GetCoordinateDistrictByCode")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCoordinateDistrictByCode(string code)
        {
            if (code == null) return BadRequest();

            var result = JObject.Parse(System.IO.File.ReadAllText("map.json"))["districts"]
                            .Where(x => x["code"].Value<string>() == code)
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"],
                                parentCode = n["parentCode"],
                                nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                                nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                                coordinates = n["coordinates"]
                                //path = n["path"]
                            }).FirstOrDefault();

            return Ok(result);
        }

        [HttpPost("GetCoordinateWardByCode")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCoordinateWardByCode(string code)
        {
            if (code == null) return BadRequest();

            var result = JObject.Parse(System.IO.File.ReadAllText("map.json"))["wards"]
                            .Where(x => x["code"].Value<string>() == code)
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"],
                                nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                                nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                                parentCode = n["parentCode"],
                                coordinates = n["coordinates"]
                                //path = n["path"]
                            }).FirstOrDefault();

            return Ok(result);
        }

        // Get Coordinates by name
        [HttpPost("GetCoordinateProvinceByName")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCoordinateProvinceByName(string nameWithType)
        {
            if (nameWithType == null) return BadRequest();

            var result = JObject.Parse(System.IO.File.ReadAllText("map.json"))["cities"]
                            .Where(n => n["nameWithType"].Value<string>().ToLower().Contains(nameWithType.ToLower()))
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"],
                                nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                                nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                                coordinates = n["coordinates"]
                            }).FirstOrDefault();

            return Ok(result);
        }

        [HttpPost("GetCoordinateDistrictByName")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCoordinateDistrictByName(string nameWithType)
        {
            if (nameWithType == null) return BadRequest();

            var result = JObject.Parse(System.IO.File.ReadAllText("map.json"))["districts"]
                            .Where(n => n["nameWithType"].Value<string>().ToLower().Contains(nameWithType.ToLower()))
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"],
                                nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                                nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                                coordinates = n["coordinates"]
                                //path = n["path"]
                            }).FirstOrDefault();

            return Ok(result);
        }

        [HttpPost("GetCoordinateWardByName")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCoordinateWardByName(string nameWithType)
        {
            if (nameWithType == null) return BadRequest();

            var result = JObject.Parse(System.IO.File.ReadAllText("map.json"))["wards"]
                            .Where(n => n["nameWithType"].Value<string>().ToLower().Contains(nameWithType.ToLower()))
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"],
                                nameEn = _convertVietNameseService.ConvertVietNamese(n["name"].ToString()),
                                nameWithTypeEn = _convertVietNameseService.ConvertAdministrativedViToEn(n["nameWithType"].ToString()),
                                coordinates = n["coordinates"]
                                //path = n["path"]
                            }).FirstOrDefault();

            return Ok(result);
        }
    }
}
