using Feedback.APIs.Endpoints.Contracts;
using Feedback.APIs.Persistence;
using Feedback.APIs.Serivces;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feedback.APIs.Endpoints;

public static class SubjectEndpoints
{
    public static void MapSubjectEndpoint(this IEndpointRouteBuilder endpoint)
    {
        var group = endpoint.MapGroup("/subject");

        group.MapPost("/", CreateSubject);
        group.MapGet("/{id}/check", CheckSubjectForReview);
        group.MapPost("/review/ranking/{id}", GetRanking);
        group.MapPost("/review", CreateReview);
        group.MapPost("/{id}/review", GetAllReviews);
    }

    public async static Task<Results<ValidationProblem, Created>> CreateSubject(
            SubjectService subjectService,
            IUserPrincipals userPrincipals,
            IValidator<CreateSubjectRequest> validator,
            IConfiguration configuration,
            CreateSubjectRequest request)
    {
        var validate = validator.Validate(request);
        if (!validate.IsValid)
            return TypedResults.ValidationProblem(validate.ToDictionary());

        var subjectId = await subjectService.Create(request.Title, request.TenantId, request.ExpiredOn);
        //TODO: make url shorter by shortner url service and return that.

        return TypedResults.Created($"{configuration["BaseUrl"]}/subject/{subjectId}/");
    }

    public async static Task<Results<NotFound, BadRequest<string>, Ok>> CheckSubjectForReview(
            SubjectService subjectService,
            [FromRoute] int id,
            IConfiguration configuration)
    {
        try
        {
            await subjectService.CheckSubjectForReview(id);
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public async static Task<Results<ValidationProblem, BadRequest<string>, NotFound, Ok>> CreateReview(
            SubjectService subjectService,
            CreateReviewRequest request,
            IValidator<CreateReviewRequest> validator,

            IConfiguration configuration)
    {
        var validate = validator.Validate(request);

        if (!validate.IsValid)
            return TypedResults.ValidationProblem(validate.ToDictionary());

        try
        {
            await subjectService.AddReview(request.SubjectId, request.ReviewerName, request.Comment, request.Rate);
            return TypedResults.Ok();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }

    }
    public async static Task<Results<NotFound, Ok<double>>> GetRanking(
          FeedbackDbContext dbContext,
          [FromRoute] int id)
    {
        var subject = await dbContext
            .Subjects
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (subject is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(subject.Reviews.Average(x => x.Rate));
    }
    public async static Task<Results<NotFound, Ok<List<ReviewResponse>>>> GetAllReviews(
          FeedbackDbContext dbContext,
          [FromRoute] int id)
    {
        var subject = await dbContext
            .Subjects
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (subject is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(subject.Reviews.Select(x => new ReviewResponse
        {
            Comment = x.Comment
        }).ToList());
    }
}

