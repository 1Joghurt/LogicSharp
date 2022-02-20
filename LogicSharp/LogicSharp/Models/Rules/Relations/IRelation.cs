
namespace LogicSharp.Models
{
    public interface IRelation
    {
        public RelationType GetRelationType();
        public IRelation GetFirstMember();
        public IRelation GetSecondMember();
        public enum RelationType
        {
            PREDICATE,
            AND,
            OR,
        }
    }
}
