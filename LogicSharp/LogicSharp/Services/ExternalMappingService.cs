using LogicSharp.Services.Models;
using LogicSharp.Models;
using static LogicSharp.Services.QueryService;

namespace LogicSharp.Services
{
    public class ExternalMappingService
    {
        private readonly Dictionary<Variable, Variable> inputMapping = new();

        public List<object> MapToInternal(Predicate predicate)
        {
            for (int i = 0; i < predicate.Attributes.Count; i++)
            {
                if (Variable.IsVariable(predicate.Attributes[i]))
                {
                    var baseVariable = (Variable)predicate.Attributes[i];
                    var replacementVariable = new Variable("_internal_" + baseVariable.Name);
                    inputMapping.Add(replacementVariable, baseVariable);
                    predicate.Attributes[i] = replacementVariable;
                }
            }
            return predicate.Attributes;
        }

        public List<object> MapToExternel(Predicate predicate)
        {
            for (int i = 0; i < predicate.Attributes.Count; i++)
            {
                if (Variable.IsVariable(predicate.Attributes[i]))
                {
                    var replacementVariable = (Variable)predicate.Attributes[i];
                    if (inputMapping.TryGetValue(replacementVariable, out var baseVariable))
                    {
                        predicate.Attributes[i] = baseVariable;
                    }
                }
            }
            return predicate.Attributes;
        }

        public List<ResultSet> GetResultSet(List<Mapping> mappings)
        {
            ResultSet[,]? ordnung = null;
            var resultSet = new List<ResultSet>();
            if (inputMapping.Count > 0)
            {
                for (var i = 0; i < inputMapping.Count; i++)
                {
                    var values = GetValues(inputMapping.ElementAt(i).Key, mappings).Select(v => new ResultSet(inputMapping.ElementAt(i).Value.ToString(), v)).ToList();
                    if (ordnung == null) ordnung = new ResultSet[values.Count, inputMapping.Count];

                    var index = 0;
                    values.ForEach((v) => ordnung[index++, i] = v);
                }

                for (int column = 0; ordnung != null && column < ordnung.GetLength(0); column++)
                {
                    for (int row = 0; row < ordnung.GetLength(1); row++)
                    {
                        resultSet.Add(ordnung[column, row]);
                    }
                }
            }
            return resultSet;
        }
    } 
}
