using MediatR;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;
using General.Application.Interfaces;
using General.Application.Common.Interfaces;
using General.Domain.Entities;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using General.Application.Common.Results;
using General.Domain.Enums;
using System.Collections.Generic;

namespace General.Application.Seller.Queries
{
    public class GetPagingSellerQuery : IRequest<PaginatedList<ProfileInformationModel>>
    {
        public PagingIndexModel PagingModel { set; get; }
    }

    public class GetPagingSellerQueryHandler : IRequestHandler<GetPagingSellerQuery, PaginatedList<ProfileInformationModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<ProfileInformationEntity> _repository;
        private readonly IIdentityService _identityService;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetPagingSellerQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            IAsyncRepository<ProfileInformationEntity> repository,
            IIdentityService identityService,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<PaginatedList<ProfileInformationModel>> Handle(GetPagingSellerQuery request, CancellationToken cancellationToken)
        {
            var model = request.PagingModel;
            string host = _commonFunctionService.ConvertImageUrl("");
            var users = (await _identityService.GetUsersInRoleAsync(Roles.Seller)).ToList();
            var result = new List<ProfileInformationModel>();

            if (users != null && users.Count() > 0)
            {
                var profiles = await _context.ProfileInformation
                        .AsNoTracking()
                        .OrderByDescending(x => x.CreateTime)
                        .ProjectTo<ProfileInformationModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

                var propertySellers = await _context.PropertySeller.ToListAsync();

                for (int i = 0; i < users.Count(); i++)
                {
                    var profile = profiles.Where(x => x.UserId.ToString() == users[i].Id).FirstOrDefault();

                    if (profile != null)
                    {
                        profile.AvatarUrl = !string.IsNullOrEmpty(profile.Avatar) ? host + profile.Avatar : "";
                        profile.PropertyCount = propertySellers.Where(x => x.UserId == profile.UserId).ToList().Count();
                        profile.Email = users[i].Email;
                        profile.UserName = users[i].UserName;
                
                        result.Add(profile);
                    }
                }
            }

            if (!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageSize == 0 || model.PageNumber == 0)
            {
                return new PaginatedList<ProfileInformationModel>(result, result.Count, 1, result.Count);
            }

            var paginatedList = PaginatedList<ProfileInformationModel>.Create(result, model.PageNumber.Value, model.PageSize.Value);

            return paginatedList;
        }
    }
}
