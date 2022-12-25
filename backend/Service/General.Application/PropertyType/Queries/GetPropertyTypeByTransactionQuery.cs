using General.Domain.Models;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.PropertyType.Queries
{
    public class GetPropertyTypeByTransactionQuery : IRequest<List<PropertyTypeModel>>
    {
        public string TransactionId { set; get; }
    }

    public class GetPropertyTypeByTransactionQueryHandler : IRequestHandler<GetPropertyTypeByTransactionQuery, List<PropertyTypeModel>>
    {
        public GetPropertyTypeByTransactionQueryHandler () { }

        public async Task<List<PropertyTypeModel>> Handle(GetPropertyTypeByTransactionQuery request, CancellationToken cancellationToken)
        {
            string masterData = "MasterData.json";
            List<PropertyTypeModel> propertyTypes = new List<PropertyTypeModel>();

            var jsonPropertyType = JObject.Parse(File.ReadAllText(masterData))["propertyType"]
                .Select(n => new {
                    id = n["id"],
                    propertyTypeVi = n["propertyTypeVi"],
                    propertyTypeEn = n["propertyTypeEn"]
                }).ToArray();

            if (!string.IsNullOrEmpty(request.TransactionId))
            {
                var jsonTransactionType = JObject.Parse(File.ReadAllText(masterData))["transactionType"]
                    .Where(n => n["id"].Value<string>() == request.TransactionId)
                    .Select(n => new {
                        propertyTypes = n["propertyTypes"]
                    });

                if (jsonTransactionType != null)
                {
                    var propertyTypeArray = jsonTransactionType.FirstOrDefault().propertyTypes.ToArray();

                    if (propertyTypeArray.Count() > 0)
                    {
                        for (int i = 0; i < propertyTypeArray.Count(); i++)
                        {
                            var propertyType = jsonPropertyType.Where(x => x.id.ToString() == propertyTypeArray[i].ToString()).FirstOrDefault();
                            propertyTypes.Add(new PropertyTypeModel
                            {
                                Id = propertyType.id.ToString(),
                                PropertyTypeVi = propertyType.propertyTypeVi.ToString(),
                                PropertyTypeEn = propertyType.propertyTypeEn.ToString(),
                            });
                        }
                    }
                }
            } else
            {
                if (jsonPropertyType.Count() > 0)
                {
                    for (int i = 0; i < jsonPropertyType.Count(); i++)
                    {
                        propertyTypes.Add(new PropertyTypeModel
                        {
                            Id = jsonPropertyType[i].id.ToString(),
                            PropertyTypeVi = jsonPropertyType[i].propertyTypeVi.ToString(),
                            PropertyTypeEn = jsonPropertyType[i].propertyTypeEn.ToString(),
                        });
                    }
                }
            }

            return propertyTypes;
        }
    }
}
