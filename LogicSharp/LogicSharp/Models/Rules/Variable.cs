

namespace LogicSharp.Models
{
    public class Variable
    {
        public Variable(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public static bool IsVariable(object obj)
        {
            return obj is Variable;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(Variable)) return false;
            var variable = (Variable)obj;
            if (Name.Equals(variable.Name)) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

    }
}