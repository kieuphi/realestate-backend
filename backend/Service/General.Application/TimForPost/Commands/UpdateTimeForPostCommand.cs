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

namespace General.Application.TimeForPost.Commands
{
    public class UpdateTimeForPostCommand : IRequest<Result>
    {
        public CreateTimeForPostModel Model { set; get; }
        public Guid TimeForPostId { set; get; }
    }

    public class UpdateTimeForPostCommandHandler : IRequestHandler<UpdateTimeForPostCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateTimeForPostCommandHandler(
            IApplicationDbContext context,
            IMapper mapper
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result> Handle(UpdateTimeForPostCommand request, CancellationToken cancellationToken)
        {
            var model = request.Model;
            var entity = await _context.TimeForPost.FindAsync(request.TimeForPostId);

            if (entity == null)
            {
                return Result.Failure(new List<string> { "The specified Time For Post not exists." });
            }

            entity.Value = model.Value;
            entity.DisplayName = model.DisplayName;
            entity.Description = model.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
