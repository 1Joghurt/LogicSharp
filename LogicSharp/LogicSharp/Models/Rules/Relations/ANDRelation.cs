
namespace LogicSharp.Models
{
    public class ANDRelation : IRelation
    {
        private readonly IRelation firstMember;
        private readonly IRelation secondMember;

        public ANDRelation(IRelation firstMember, IRelation secondMember)
        {
            this.firstMember = firstMember;
            this.secondMember = secondMember;
        }

        public IRelation GetFirstMember()
        {
            return firstMember;
        }

        public IRelation GetSecondMember()
        {
            return secondMember;
        }


        IRelation.RelationType IRelation.GetRelationType()
        {
            return IRelation.RelationType.AND;
        }

        public override string ToString()
        {
            var output = "(";
            output += firstMember.ToString() + ",";
            output += secondMember.ToString() + ")";
            return output;
        }
    }
}
