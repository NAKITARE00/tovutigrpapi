using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IIssues
    {
        Task<IEnumerable<Issues>> GetAllIssues();
        Task<String> AddIssue(Issues issues);
        Task<IssueRetrieval> GetSingleIssue(int issueId);
    }
}
