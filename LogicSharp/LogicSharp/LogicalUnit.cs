using LogicSharp.Models;
using LogicSharp.Services;

namespace LogicSharp
{
    public class LogicalUnit
    {
        public List<Rule> rules = new();

        public LogicalUnit()
        { }

        public LogicalUnit(params Rule[] rules)
        {
            Rules = rules.ToList();
        }

        public List<Rule> Rules { get => rules; set => rules = value; }

        public RequestQueryResult Query(string name)
        {
            return Query(new Predicate(name));
        }

        public RequestQueryResult Query(string name, params object[] attributes)
        {
            return Query(new Predicate(name, attributes));
        }

        public RequestQueryResult Query(Predicate requestQuery)
        {
            var queryService = new QueryService(Rules, requestQuery);
            return queryService.QueryRequest();
        }

        public override string ToString()
        {
            var output = "";
            foreach (var rule in Rules)
            {
                if (!string.IsNullOrEmpty(output)) output += "\n";
                output += rule.ToString();
            }
            return output;
        }
    }
}
