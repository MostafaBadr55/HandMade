using HandMade.Application.Features.Categories.Queries.GetCategoryManagementDashboard.DTOs;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Queries.GetCategoryManagementDashboard
{
    public record GetCategoryManagementDashboardQuery(string? SearchTerm,int PageNumber = 1 ,int PageSize = 5) : IRequest<RequestResult<PagedResult<CategoryWithSubcategoriesItemDTO>>>;

    internal class GetCategoryManagementDashboardQueryHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<GetCategoryManagementDashboardQuery, RequestResult<PagedResult<CategoryWithSubcategoriesItemDTO>>>
    {
        public async Task<RequestResult<PagedResult<CategoryWithSubcategoriesItemDTO>>> Handle(GetCategoryManagementDashboardQuery request, CancellationToken cancellationToken)
        {
            var spec = new CategorySpecification(request.SearchTerm);

            var pagedResult = await _unitOfWork
                .GetRepository<Category>()
                .GetAll()
                .ApplySpecification(spec)
                .Select(c => new CategoryWithSubcategoriesItemDTO
                {
                    Id = c.Id,
                    CategoryName = c.Name,
                    CategoryDescription = c.Description,
                    CategoryImage = c.ImageUrl,
                    CreatedAt = c.CreatedAt,
                    Subcategories = c.SubCategories
                        .Where(sc => !sc.IsDeleted)
                        .Select(sc => new SubCategorItemDTO
                        {
                            Id = sc.Id,
                            SubcategoryName = sc.Name,
                            CreatedAt = sc.CreatedAt,
                            UpdateAt = sc.UpdatedAt
                        })
                        .ToList()
                })
                .ToPagedResultAsync(_executor, request.PageNumber, request.PageSize, cancellationToken);

            return RequestResult<PagedResult<CategoryWithSubcategoriesItemDTO>>.Success(pagedResult);
        }
    }
}
