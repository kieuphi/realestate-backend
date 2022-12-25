using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using AutoMapper;
using General.Domain.Entities;
using General.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Common.Shared.Enums;
using System.IO;
using Newtonsoft.Json.Linq;

namespace General.Application.Project.Queries
{
    public class GetAdministrativeByProjectQuery : IRequest<List<AdministrativeByProjectModel>>
    {
    }

    public class GetAdministrativeByProjectQueryHandler : IRequestHandler<GetAdministrativeByProjectQuery, List<AdministrativeByProjectModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ProjectEntity> _repository;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetAdministrativeByProjectQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IAsyncRepository<ProjectEntity> repository,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<AdministrativeByProjectModel>> Handle(GetAdministrativeByProjectQuery request, CancellationToken cancellationToken)
        {
            List<AdministrativeByProjectModel> administrative = new List<AdministrativeByProjectModel>();
            var projects = await _context.Project.Where(x => x.IsDeleted == DeletedStatus.False).ToListAsync();
            List<string> listProvinceCode = projects.Select(x => x.ProvinceCode).Distinct().ToList();
            string mapJSON = "map.json";

            var provincesList = JObject.Parse(File.ReadAllText(mapJSON))["cities"]
                            .Select(n => new {
                                code = n["code"],
                                name = n["name"],
                                nameWithType = n["nameWithType"]
                                //coordinates = n["coordinates"]
                            }).ToArray();

            if (listProvinceCode != null && listProvinceCode.Count() > 0)
            {
                for (int i = 0; i < listProvinceCode.Count(); i++)
                {
                    var province = provincesList.Where(x => x.code.ToString() == listProvinceCode[i]).FirstOrDefault();
                    if (province != null)
                    {
                        administrative.Add(new AdministrativeByProjectModel
                        {
                            Code = province.code.ToString(),
                            Name = province.name.ToString(),
                            NameWithType = province.nameWithType.ToString()
                        });
                    }
                }
            }

            return administrative;
        }
    }
}
