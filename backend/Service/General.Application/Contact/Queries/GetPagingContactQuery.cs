using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace General.Application.Contact.Queries
{
    public class GetPagingContactQuery : IRequest<PaginatedList<ContactModel>>
    {
        public PagingContactModel PagingModel { set; get; }
    }

    public class GetPagingContactQueryHandler : IRequestHandler<GetPagingContactQuery, PaginatedList<ContactModel>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IAsyncRepository<ContactEntity> _repository;

        public GetPagingContactQueryHandler(
            IMapper mapper,
            IApplicationDbContext context,
            IAsyncRepository<ContactEntity> repository
        )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<PaginatedList<ContactModel>> Handle(GetPagingContactQuery request, CancellationToken cancellationToken)
        {
            var model = request.PagingModel;
            var contact = await _repository
                        .WhereIgnoreDelete(null)
                        .AsNoTracking()
                        .OrderByDescending(x => x.CreateTime)
                        .ProjectTo<ContactModel>(_mapper.ConfigurationProvider)
                        .ToListAsync();

            if(!model.PageNumber.HasValue || !model.PageSize.HasValue || model.PageNumber == 0 || model.PageSize == 0)
            {
                return new PaginatedList<ContactModel>(contact, contact.Count, 1, contact.Count);
            }

            var paginatedList = PaginatedList<ContactModel>.Create(contact, model.PageNumber.Value, model.PageSize.Value);

            return paginatedList;
        }
    }
}
