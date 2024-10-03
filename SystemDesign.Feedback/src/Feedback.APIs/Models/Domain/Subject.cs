namespace Feedback.APIs.Models.Domain;

public class Subject
{
    public int Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool Locked { get; set; }

    public DateTime? ExpiredOn { get; set; }

    public string Title { get; set; }

    public int TenantId { get; set; }

    public ICollection<Review> Reviews { get; set; }

    public static Subject Create(
        string title,
        int tenantId,
        DateTime? expiredOn
        )
        => new Subject
        {
            Locked = false,
            ExpiredOn = expiredOn,
            Title = title,
            TenantId = tenantId,
        };

    public void AddReview(
        int rate,
        string comment,
        string reviewerName
        )
    {
        Reviews.Add(new Review
        {
            Comment = comment,
            ReviewerName = reviewerName

        });
    }
}
