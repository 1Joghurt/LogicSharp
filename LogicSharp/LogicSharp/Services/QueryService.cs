using LogicSharp.Models;
using LogicSharp.Services.Models;

namespace LogicSharp.Services
{
    public class QueryService
    {
        private readonly List<Rule> rules;
        private readonly Predicate requestQuery;

        public QueryService(List<Rule> rules, Predicate requestQuery)
        {
            this.rules = rules;
            this.requestQuery = requestQuery;
        }
        public RequestQueryResult QueryRequest()
        {
            var externalMappingService = new ExternalMappingService();
            var result = new RequestQueryResult( requestQuery );

            requestQuery.Attributes = externalMappingService.MapToInternal(requestQuery);

            var queryResult = Resolve(requestQuery, new List<Mapping>(), new List<ProcessedMapping>());
            if (queryResult.Successfull)
            {
                result.Result = true;
                result.ResultSets = externalMappingService.GetResultSet(queryResult.Mappings);
            }

            requestQuery.Attributes = externalMappingService.MapToExternel(requestQuery);
            return result;
        }

        private SubGoalResult Resolve(IRelation relation, List<Mapping> mappings, List<ProcessedMapping> processedMappings)
        {
            var result = new SubGoalResult();

            if (relation.GetRelationType() == IRelation.RelationType.PREDICATE)
            {
                Predicate predicate = (Predicate)relation;
                var matchingRules = GetMatchingRules(predicate, mappings);

                foreach (var ruleObj in matchingRules)
                {
                    var rule = ruleObj.Key;
                    var unification = ruleObj.Value;

                    if (rule.IsFact())
                    {
                        if (!Contains(unification, predicate, processedMappings))
                        {
                            result.Successfull = true;
                            result.Mappings.AddRange(unification);
                            processedMappings.Add(new ProcessedMapping(predicate, unification));
                        }
                    }
                    else
                    {
                        SubGoalResult subResult;
                        do
                        {
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
                            subResult = Resolve(rule.Body, unification, processedMappings);
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
                            if (subResult.Successfull)
                            {
                                result.Successfull = true;
                                result.Mappings.AddRange(subResult.Mappings);
                                result.Mappings.AddRange(unification);
                            }
                        }
                        while (subResult.Successfull);
                    }
                }
            }
            else if (relation.GetRelationType() == IRelation.RelationType.OR)
            {

                SubGoalResult resultFirst;
                var resultSecond = new SubGoalResult();
                do
                {

                    resultFirst = Resolve(relation.GetFirstMember(), mappings, processedMappings);
                    if (resultFirst.Successfull)
                    {
                        result.Successfull = true;
                        result.Mappings.AddRange(resultFirst.Mappings);
                    }
                    else
                    {
                        resultSecond = Resolve(relation.GetSecondMember(), mappings, processedMappings);
                        if (resultSecond.Successfull)
                        {
                            result.Successfull = true;
                            result.Mappings.AddRange(resultSecond.Mappings);
                        }
                    }
                }
                while (resultFirst.Successfull || resultSecond.Successfull);
            }
            else // AND
            {
                SubGoalResult resultFirst;
                do
                {
                    resultFirst = Resolve(relation.GetFirstMember(), mappings, processedMappings);
                    if (resultFirst.Successfull)
                    {
                        SubGoalResult resultSecond;
                        do
                        {
                            resultSecond = Resolve(relation.GetSecondMember(), resultFirst.Mappings, processedMappings);
                            if (resultSecond.Successfull)
                            {
                                result.Successfull = true;
                                result.Mappings.AddRange(resultSecond.Mappings);
                            }
                        }
                        while (resultSecond.Successfull);
                    }
                }
                while (resultFirst.Successfull);
            }
            return result;
        }

        private Dictionary<Rule, List<Mapping>> GetMatchingRules(Predicate predicate, List<Mapping> mappings)
        {
            var matchingRules = new Dictionary<Rule, List<Mapping>>();
            var sameFormRules = rules.FindAll(x => x.Head.Name == predicate.Name && x.Head.Attributes.Count == predicate.Attributes.Count);
            foreach (var rule in sameFormRules)
            {
                var unificationResult = Unificate(predicate, rule.Head, mappings);
                if (unificationResult.Successfull)
                {
                    matchingRules.Add(rule, unificationResult.Mappings);
                }
            }
            return matchingRules;
        }

        private static SubGoalResult Unificate(Predicate basePredicate, Predicate secondPredicate, List<Mapping> mappings)
        {
            var result = new SubGoalResult();

            for (var i = 0; i < basePredicate.Attributes.Count; i++)
            {
                if (Variable.IsVariable(basePredicate.Attributes[i]))
                {
                    var baseVariable = (Variable)basePredicate.Attributes[i];
                    var baseVariableValues = GetValues(baseVariable, mappings);

                    if (Variable.IsVariable(secondPredicate.Attributes[i]))
                    {
                        if (!baseVariableValues.Any())
                        {
                            result.Mappings.Add(new Mapping((Variable)secondPredicate.Attributes[i], basePredicate.Attributes[i]));
                        }
                        else
                        {
                            var list = new List<object>();
                            var toGet = new List<Variable> { (Variable)basePredicate.Attributes[i] };
                            var verarbeitet = new List<Variable>();

                            do
                            {
                                var values = GetValues(toGet[0], mappings);
                                values.ForEach(value =>
                                {

                                    if (Variable.IsVariable(value))
                                    {
                                        if (!verarbeitet.Contains((Variable)value)) toGet.Add((Variable)value);
                                    }
                                    else
                                    {
                                        list.Add(value);
                                    }
                                });
                                verarbeitet.Add(toGet[0]);
                                toGet.RemoveAt(0);
                            }
                            while (toGet.Count > 0);

                            list.ForEach(v => result.Mappings.Add(new Mapping((Variable)secondPredicate.Attributes[i], v)));
                        }

                        result.Successfull = true;
                    }
                    else if (baseVariableValues.Any())
                    {
                        if (!baseVariableValues.Contains(secondPredicate.Attributes[i]))
                        {
                            result.Successfull = false;
                            return result;
                        }
                        else
                        {
                            result.Mappings.Add(new Mapping((Variable)basePredicate.Attributes[i], secondPredicate.Attributes[i]));
                        }
                    }
                    else
                    {
                        result.Mappings.Add(new Mapping(baseVariable, secondPredicate.Attributes[i]));
                    }
                }
                else
                {
                    if (Variable.IsVariable(secondPredicate.Attributes[i]))
                    {
                        result.Mappings.Add(new Mapping((Variable)secondPredicate.Attributes[i], basePredicate.Attributes[i]));
                    }
                    else
                    {
                        if (!basePredicate.Attributes[i].Equals(secondPredicate.Attributes[i]))
                        {
                            result.Successfull = false;
                            return result;
                        }
                    }
                }
            }
            result.Successfull = true;
            return result;
        }

        public static List<object> GetValues(Variable variable, List<Mapping> mappings)
        {
            var values = new List<object>();
            var variableList = new List<Variable> { variable };
            var processedVariableList = new List<Variable> { variable };

            while (variableList.Count > 0)
            {
                for (int i = 0; i < variableList.Count; i++)
                {
                    mappings.ForEach(mapping =>
                    {
                        if (mapping.Variable.Equals(variableList[i]))
                        {
                            if (Variable.IsVariable(mapping.MappedValue))
                            {
                                var mappedVariable = (Variable)mapping.MappedValue;
                                if (!processedVariableList.Contains(mappedVariable))
                                {
                                    processedVariableList.Add(mappedVariable);
                                    variableList.Add(mappedVariable);
                                }
                            }
                            else
                            {
                                values.Add(mapping.MappedValue);
                            }
                        }
                        else if (mapping.MappedValue.Equals(variableList[i]))
                        {
                            if (!processedVariableList.Contains(mapping.Variable))
                            {
                                processedVariableList.Add(mapping.Variable);
                                variableList.Add(mapping.Variable);
                            }
                        }
                    });
                    variableList.RemoveAt(i);
                }
            }
            return values;
        }

        private static bool Contains(List<Mapping> mappings, Predicate predicate, List<ProcessedMapping> processedMapping)
        {
            var sameCountMappings = processedMapping.FindAll(fm => fm.Predicate.Equals(predicate) && fm.Mappings.Count == mappings.Count);

            foreach (var mapping in sameCountMappings)
            {
                bool sameList = true;
                foreach (var m in mappings)
                {
                    if (!mapping.Mappings.Contains(m))
                    {
                        sameList = false;
                        break;
                    }
                }
                if (sameList) return true;
            }
            return false;
        }
    }
}