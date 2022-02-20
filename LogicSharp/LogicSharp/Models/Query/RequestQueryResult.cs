using LogicSharp.Models;
using LogicSharp.Services.Models;

namespace LogicSharp.Models
{
    public class RequestQueryResult
    {
        private List<ResultSet> resultSets = new();

        public RequestQueryResult(Predicate query)
        {
            Query = query;
        }

        public Predicate Query { get; set; }

        public bool Result { get; set; }

        public List<ResultSet> ResultSets { get => resultSets; set => resultSets = value; }

        public override string ToString()
        {
            var output = "Query:\t" + Query.ToString() + "\nResult:\t" + Result;
            if (ResultSets != null && ResultSets.Count > 0)
            {
                output += "\nValues:\t";
                foreach (ResultSet resultSet in ResultSets)
                {
                    output += $"({resultSet.Name}={resultSet.Value}) ";
                }
            }
            return output;
        }
    }
}
