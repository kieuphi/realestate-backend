using AutoMapper;
using AutoMapper.QueryableExtensions;
using General.Application.Common.Results;
using General.Application.Interfaces;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Supplier.Queries
{
    public class GetSupplierByIdQuery : IRequest<ProfileInformationModel>
    {
        public Guid UserId { set; get; }
    }

    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, ProfileInformationModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetSupplierByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IIdentityService identityService,
            ICommonFunctionService commonFunctionService
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<ProfileInformationModel> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");
            var supplier = await _context.ProfileInformation
                            .Where(x => x.UserId == request.UserId)
                            .AsNoTracking()
                            .ProjectTo<ProfileInformationModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync();

            var user = _mapper.Map<UserResult>(await _identityService.GetUserByIdentifierAsync(supplier.UserId.ToString()));
            if(user != null)
            {
                supplier.Email = user.Email;
                supplier.UserName= user.UserName;
            }

            supplier.AvatarUrl = !string.IsNullOrEmpty(supplier.Avatar) ? host + supplier.Avatar : "";

            // social network
            var socialNetworkUsers = _context.SocialNetworkUser.Where(x => x.ProfileId == supplier.Id).ProjectTo<SocialNetworkUserModel>(_mapper.ConfigurationProvider).ToList();
            if (socialNetworkUsers != null)
            {
                supplier.SocialNetworks = socialNetworkUsers;
                var socialNetworks = _context.SocialNetwork.ToList();

                for (int i = 0; i < supplier.SocialNetworks.Count(); i++)
                {
                    for(int j = 0; j < socialNetworks.Count(); j++)
                    {
                        if(supplier.SocialNetworks[i].SocialNetworkId == socialNetworks[j].Id)
                        {
                            supplier.SocialNetworks[i].SocialNetworkName = socialNetworks[j].AppName;
                            supplier.SocialNetworks[i].SocialNetwokIcon = socialNetworks[j].ICon;

                            break;
                        }
                    }
                }
            }

            return supplier;
        }
    }
}
