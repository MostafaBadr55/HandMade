using HandMade.Application.Features.Categories.Queries.GetCategoryCards.DTOs;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Queries.GetCategoryCards
{
    public record GetCategoryCardsQuery() : IRequest<RequestResult<PagedResult<CategoryCardDTO>>>;

    public class GetCategoryCardsQueryHandler(IUnitOfWork unitOfWork, IQueryableExecutor executor, IUrlBuilder urlBuilder)
        : IRequestHandler<GetCategoryCardsQuery, RequestResult<PagedResult<CategoryCardDTO>>>
    {
        public async Task<RequestResult<PagedResult<CategoryCardDTO>>> Handle(GetCategoryCardsQuery request, CancellationToken cancellationToken)
        {

            var query = unitOfWork.GetRepository<Category>()
                                  .GetAll()
                                  .Select(c => new CategoryCardDTO
                                  {
                                      CategoryId = c.Id,
                                      Name = c.Name,
                                      Description = c.Description,
                                      CategoryImage = c.ImageUrl
                                  });

            var categories = await query.ToPagedResultAsync(executor,1, 20);

            foreach(var item in categories.Items)
            {
                if(item.CategoryImage is not null)
                urlBuilder.BuildAbsoluteUrl(item.CategoryImage);
            }

            return RequestResult<PagedResult<CategoryCardDTO>>.Success(categories);
        }
    }

}
