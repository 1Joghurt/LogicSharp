
namespace LogicSharp.Models
{
    public class Rule 
    {
        public Rule(Predicate head, IRelation? body = null)
        {
            Head = head;
            Body = body;
        }

        public Predicate Head { get; set; }

        public IRelation? Body { get; set; }

        public bool IsFact()
        { 
            return Body == null;
        }

        public override string ToString()
        {
            var output = Head.ToString();

            if (Body != null)
            {
                output += ":-" + Body.ToString() ;
            }
            return output + ".";
        }
    }
}
