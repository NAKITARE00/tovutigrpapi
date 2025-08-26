using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IIssues
    {
        Task<IEnumerable<Issues>> GetAllIssues(int staff_id);
        Task<String> AddIssue(Issues issues, int staff_id);
        Task<IssueRetrieval> GetSingleIssue(int issueId, int staff_id);
        Task<string> UpdateIssue(Issues issue, int staff_id);
        Task<string> DeleteIssue(int issueId, int staff_id);
        Task<IEnumerable<Issues>> GetAllIssuesAnalytics();
    }
}
