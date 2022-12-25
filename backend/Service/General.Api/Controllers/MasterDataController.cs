using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Shared.Models;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using General.Domain.Models;
using General.Application.PropertyType.Queries;

namespace General.Api.Controllers
{
    public class MasterDataController : ApiController
    {
        private readonly ILogger<MasterDataController> _logger;

        public MasterDataController(ILogger<MasterDataController> logger)
        {
            _logger = logger;   
        }

        //// GET: api/masterData
        [HttpGet("GetMasterData")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status402PaymentRequired)]
        public object GetMasterData()
        {
            string json = System.IO.File.ReadAllText("MasterData.json");
            dynamic result = new JsonResult(Newtonsoft.Json.JsonConvert.DeserializeObject(json)).Value;

            return result;
        }

        //// GET: api/masterData
        [HttpGet("GetPropertyTypeByTransaction")]
        [ProducesResponseType(typeof(List<PropertyTypeModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<List<PropertyTypeModel>> GetPropertyTypeByTransaction(string transactionId)
        {
            var result = await Mediator.Send(new GetPropertyTypeByTransactionQuery() { TransactionId = transactionId });

            return result;
        }
    }
}
