using Feedback.APIs.Models.Domain;
using Feedback.APIs.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Feedback.APIs.Serivces;

public class SubjectService(FeedbackDbContext dbContext)
{
    private readonly FeedbackDbContext dbContext = dbContext;


    public async Task<int> Create(string title, int tenantId, DateTime? expiredOn)
    {
        if (dbContext.Subjects.Any(x => x.TenantId == tenantId && x.Title == title))
            throw new Exception();

        var subject = Subject.Create(title, tenantId, expiredOn);
        dbContext.Add(subject);
        await dbContext.SaveChangesAsync();
        return subject.Id;

    }


    public async Task CheckSubjectForReview(int id)
    {
        var subject = await dbContext.Subjects.FirstOrDefaultAsync(x => x.Id == id);
        CheckActionForReview(subject);

    }

    private static void CheckActionForReview(Subject? subject)
    {
        if (subject is null)
            throw new Exception("Entity not found");

        if (subject.Locked)
            throw new Exception("Entity lockec");


        if (subject.ExpiredOn is not null && subject.ExpiredOn > DateTime.Now)
            throw new Exception("Entity expired");
    }

    internal async Task AddReview(int subjectId, string reviewerName, string comment, int rate)
    {
        var subject = await dbContext
            .Subjects
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == subjectId);

        CheckActionForReview(subject);

        subject?.AddReview(rate, comment, reviewerName);
        await dbContext.SaveChangesAsync();
    }
}
