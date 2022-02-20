
namespace LogicSharp.Models
{
    public class Predicate : IRelation
    {
        private List<object> attributes = new();

        public Predicate(string name)
        {
            Name = name;
            Attributes = new List<object>();
        }

        public Predicate(string name, params object[] attributes)
        {
            Name = name;
            Attributes = attributes.ToList();
        }

        public string Name { get; set; }
        public List<object> Attributes { get  => attributes ; set => attributes = value; }

        public IRelation GetFirstMember()
        {
            return this;
        }

        public IRelation GetSecondMember()
        {
            return this;
        }

        IRelation.RelationType IRelation.GetRelationType()
        {
            return IRelation.RelationType.PREDICATE;
        }

        public override string ToString()
        {
            var output = Name + "(";
            var attributesString = "";

            foreach (var attribute in Attributes)
            {
                if (!string.IsNullOrEmpty(attributesString)) attributesString += ",";
                attributesString += attribute.ToString();
            }
            output += attributesString + ")";
            return output;
        }
    }
}
