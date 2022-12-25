using General.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Common.Shared.Models;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using AutoMapper;

namespace General.Application.SocialNetwork.Commands
{
    public class UpdateSocialNetworkCommand : IRequest<Result>
    {
        public CreateSocialNetworkModel Model { set; get; }
        public Guid SocialNetworkId { set; get; }
    }

    public class UpdateSocialNetworkCommandHandler : IRequestHandler<UpdateSocialNetworkCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateSocialNetworkCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(UpdateSocialNetworkCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var entity = await _context.SocialNetwork.FindAsync(request.SocialNetworkId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Social Network not exists." });
            }

            entity.AppName = model.AppName;
            entity.ICon = model.ICon;
            entity.Descriptions = model.Descriptions;
            entity.IsShowFooter = model.IsShowFooter;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
