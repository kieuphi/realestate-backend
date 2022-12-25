using AutoMapper;
using System;
using Models = General.Domain.Models;
using Entities = General.Domain.Entities;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using General.Application.Interfaces;
using General.Domain.Models;
using System.Linq;
using Common.Shared.Models;

namespace General.Application.Attachments.Commands
{
    public class AddAttachmentCommand : IRequest<Result>
    {
        public Models.AttachmentModel Model { get; set; }
        public string AttachmentType { get; set; }
        public string ImageCategory { get; set; }
    }

    public class AddAttachmentCommandHandler : IRequestHandler<AddAttachmentCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICommonFunctionService _commonFunctionService;

        public AddAttachmentCommandHandler(
            IApplicationDbContext context, 
            IMapper mapper,
            ICommonFunctionService commonFunctionService
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _commonFunctionService = commonFunctionService ?? throw new ArgumentNullException(nameof(commonFunctionService));
        }

        public async Task<Result> Handle(AddAttachmentCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Entities.AttachmentEntity>(request.Model);
            entity.AttachmentType = null;

            Entities.AttachmentTypeEntity attachmentType = attachmentType = _context.AttachmentType.FirstOrDefault(x => x.Name == request.AttachmentType);
            Entities.ImageCategoryEntity imageCategory = imageCategory = _context.ImageCategory.FirstOrDefault(x => x.Name == request.ImageCategory);

            if (attachmentType == null)
            {
                return Result.Failure("AttachmentType not exist");
            }

            entity.AttachmentTypeId = attachmentType.Id;
            entity.GroupId = Guid.NewGuid();
            entity.ImageCategoryId = imageCategory.Id;

            _context.Attachment.Add(entity);
            await _context.SaveChangesAsync();

            var result = new CommonAttachmentModel()
            {
                Id = entity.Id,
                FilePath = entity.FilePath,
                FileUrl = _commonFunctionService.ConvertImageUrl(entity.FilePath)
            };

            return Result.Success(result);
            
        }
    }
}
