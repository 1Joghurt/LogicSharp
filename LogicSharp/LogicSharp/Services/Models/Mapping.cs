using LogicSharp.Models;

namespace LogicSharp.Services.Models
{
    public class Mapping
    {
        public Mapping(Variable variable, Object mappedValue)
        {
            Variable = variable;
            MappedValue = mappedValue;
        }

        public Variable Variable { get; set; }
        public object MappedValue { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(Mapping)) return false;
            Mapping mapping = (Mapping)obj;
            if (Variable.Equals(mapping.Variable) && MappedValue.Equals(mapping.MappedValue)) return true;
            return false;
        }

        public override string ToString()
        {
            return "Var:" + Variable.ToString() + " Mapped:" + MappedValue.ToString();
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Variable.GetHashCode(), MappedValue.GetHashCode());
        }
    }
}
