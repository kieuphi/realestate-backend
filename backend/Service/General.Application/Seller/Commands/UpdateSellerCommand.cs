using MediatR;
using System;
using System.Collections.Generic;
using Common.Shared.Models;
using General.Domain.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using System.Linq;
using General.Domain.Entities;

namespace General.Application.Seller.Commands
{
    public class UpdateSellerCommand : IRequest<Result>
    {
        public Guid ProfileId { set; get; }
        public UpdateProfileInformationModel Model { set; get; }
    }

    public class UpdateSellerCommandHandler : IRequestHandler<UpdateSellerCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public UpdateSellerCommandHandler(
            IApplicationDbContext context,
            IMediator mediator
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Result> Handle(UpdateSellerCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var entity = await _context.ProfileInformation.FindAsync(request.ProfileId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Seller not exists." });
            }

            entity.Avatar = model.Avatar;
            entity.FirstName = model.FirstName;
            entity.LastName = model.LastName;
            entity.PhoneNumber1 = model.PhoneNumber1;
            entity.PhoneNumber2 = model.PhoneNumber2;
            entity.PhoneNumber3 = model.PhoneNumber3;
            entity.TitleVi = model.TitleVi;
            entity.TitleEn = model.TitleEn;
            entity.TitleDescriptionVi = model.TitleDescriptionVi;
            entity.TitleDescriptionEn = model.TitleDescriptionEn;
            entity.Agency = model.Agency;

            CreateSocialNetworkForUser(model, request.ProfileId);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private void CreateSocialNetworkForUser(UpdateProfileInformationModel model, Guid sellerId)
        {
            if (model.SocialNetworks != null && model.SocialNetworks.Count() > 0)
            {
                var socialNetworkUser = _context.SocialNetworkUser.Where(x => x.ProfileId == sellerId);
                _context.SocialNetworkUser.RemoveRange(socialNetworkUser);

                foreach (var item in model.SocialNetworks)
                {
                    _context.SocialNetworkUser.Add(new SocialNetworkUserEntity
                    {
                        Id = Guid.NewGuid(),
                        ProfileId = sellerId,
                        SocialNetworkId = item.SocialNetworkId
                    });
                };
            }
            else
            {
                var socialNetworkUser = _context.SocialNetworkUser.Where(x => x.ProfileId == sellerId);
                _context.SocialNetworkUser.RemoveRange(socialNetworkUser);
            }
        }
    }
}
