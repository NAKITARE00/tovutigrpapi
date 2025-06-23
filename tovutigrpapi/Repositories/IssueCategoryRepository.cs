using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Repositories
{
    public class IssueCategoryRepository : IIssues
    {
        private readonly DataContext _dataContext;
        public IssueCategoryRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<string> AddIssue(Issues issues)
        {
            string sql = @"
                INSERT INTO Issues (Name, Issue, Gadget_Id)
                VALUES (@Name, @Issue, @Gadget_Id);
            ";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    issues.Name,
                    issues.Issue,
                    issues.Gadget_Id
                });
                if (result > 0)
                {
                    return "Issue added successfully.";
                }
                else
                {
                    return "Failed to add issue.";
                }
            }
        }

        public async Task<IEnumerable<Issues>> GetAllIssues()
        {
            string sql = "SELECT * FROM Issues";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<Issues>(sql);
            }
        }

        public async Task<IssueRetrieval> GetSingleIssue(int issueId)
        {
            string sql = @"
            SELECT 
                i.Id AS IssueId,
                i.Name,
                i.Issue,
                i.Gadget_Id,
                g.Name AS GadgetName
            FROM Issues i
            JOIN Gadgets g ON i.Gadget_Id = g.Id
            WHERE i.Id = @IssueId;
            ";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<IssueRetrieval>(sql, new { IssueId = issueId });
            }
        }
    }
}
