namespace LogicSharp.Models
{
    public class ORRelation : IRelation
    {
        private readonly IRelation firstMember;
        private readonly IRelation secondMember;

        public ORRelation(IRelation firstMember, IRelation secondMember)
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
            return IRelation.RelationType.OR;
        }
        public override string ToString()
        {
            var output = "(";
            output += firstMember.ToString() + ";";
            output += secondMember.ToString() + ")";
            return output;
        }
    }
}
