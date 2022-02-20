using LogicSharp.Models;

namespace LogicSharp.Services.Models
{
    public class ProcessedMapping
    {
        public ProcessedMapping(Predicate predicate, List<Mapping> mappings)
        {
            Predicate = predicate;
            Mappings = mappings;
        }

        public List<Mapping> Mappings { get; set; }
        public Predicate Predicate { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(ProcessedMapping)) return false;
            ProcessedMapping processedMapping = (ProcessedMapping)obj;
            if (Mappings.Equals(processedMapping.Mappings) && Predicate.Equals(processedMapping.Predicate)) return true;
            return false;
        }
        public override string ToString()
        {
            return "Predicate : " + Predicate.ToString() + " MappedValues: " + Mappings.Count;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Predicate, Mappings);
        }
    }
}
