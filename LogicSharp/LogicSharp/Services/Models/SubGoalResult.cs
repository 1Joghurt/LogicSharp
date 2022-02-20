
namespace LogicSharp.Services.Models
{
    public class SubGoalResult
    {
        private List<Mapping> mappings = new();

        public bool Successfull { get; set; }

        public List<Mapping> Mappings { get => mappings; set => mappings = value; }

    }
}
