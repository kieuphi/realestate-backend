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

namespace General.Application.Seller.Queries
{
    public class GetSellerByIdQuery : IRequest<ProfileInformationModel>
    {
        public Guid UserId { set; get; }
    }

    public class GetSellerByIdQueryHandler : IRequestHandler<GetSellerByIdQuery, ProfileInformationModel>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ICommonFunctionService _commonFunctionService;

        public GetSellerByIdQueryHandler(
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

        public async Task<ProfileInformationModel> Handle(GetSellerByIdQuery request, CancellationToken cancellationToken)
        {
            string host = _commonFunctionService.ConvertImageUrl("");
            var seller = await _context.ProfileInformation
                            .Where(x => x.UserId == request.UserId)
                            .AsNoTracking()
                            .ProjectTo<ProfileInformationModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync();

            if(seller != null)
            {
                var user = _mapper.Map<UserResult>(await _identityService.GetUserByIdentifierAsync(seller.UserId.ToString()));

                seller.Email = user.Email;
                seller.UserName= user.UserName;

                // social network
                var socialNetworkUsers = _context.SocialNetworkUser.Where(x => x.ProfileId == seller.Id).ProjectTo<SocialNetworkUserModel>(_mapper.ConfigurationProvider).ToList();
                if (socialNetworkUsers != null)
                {
                    seller.SocialNetworks = socialNetworkUsers;
                    var socialNetworks = _context.SocialNetwork.ToList();

                    for (int i = 0; i < seller.SocialNetworks.Count(); i++)
                    {
                        for (int j = 0; j < socialNetworks.Count(); j++)
                        {
                            if (seller.SocialNetworks[i].SocialNetworkId == socialNetworks[j].Id)
                            {
                                seller.SocialNetworks[i].SocialNetworkName = socialNetworks[j].AppName;
                                seller.SocialNetworks[i].SocialNetwokIcon = socialNetworks[j].ICon;

                                break;
                            }
                        }
                    }
                }

                seller.AvatarUrl = !string.IsNullOrEmpty(seller.Avatar) ? host + seller.Avatar : "";
            }


            return seller;
        }
    }
}
