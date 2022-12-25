using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Shared.Models;
using General.Application.Common.Interfaces;
using General.Application.Interfaces;
using General.Domain.Entities;
using General.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace General.Application.UserSavedSearch.Queries
{
    public class GetSortingUserSavedSearchQuery : IRequest<PaginatedList<UserSavedSearchModel>>
    {
        public SortingUserSavedSearchModel SortingUserSavedSearchModel { get; set; }
    }

    public class GetSortingUserSavedSearchQueryHandler : IRequestHandler<GetSortingUserSavedSearchQuery, PaginatedList<UserSavedSearchModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _userService;

        public GetSortingUserSavedSearchQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService userService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<PaginatedList<UserSavedSearchModel>> Handle(GetSortingUserSavedSearchQuery request, CancellationToken cancellationToken)
        {
            var sortingModel = request.SortingUserSavedSearchModel;

            var query = _context.UserSavedSearch.Where(x => x.UserId == Guid.Parse(_userService.UserId)).AsNoTracking().ProjectTo<UserSavedSearchModel>(_mapper.ConfigurationProvider);

            if (sortingModel.Newest != null && sortingModel.Newest == true)
            {
                query = query.OrderByDescending(x => x.CreateTime);
            }
            if (sortingModel.Oldest != null && sortingModel.Oldest == true)
            {
                query = query.OrderBy(x => x.CreateTime);
            }
            if (sortingModel.NameOrder != null && sortingModel.NameOrder == true)
            {
                query = query.OrderBy(x => x.Name);
            }
            if (sortingModel.NameOrderDescending != null && sortingModel.NameOrderDescending == true)
            {
                query = query.OrderByDescending(x => x.Name);
            }

            var data = await PaginatedList<UserSavedSearchModel>.CreateAsync(query, sortingModel.PageNumber, sortingModel.PageSize);
            DateTime now = DateTime.Now;

            for (int i = 0; i < data.Items.Count; i++)
            {
                data.Items[i].NumberOfDaysAgo = now.Date.Subtract(data.Items[i].CreateTime.Date).Days;
            }
            return data;
        }
    }
}
