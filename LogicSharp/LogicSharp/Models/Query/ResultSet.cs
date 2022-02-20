
namespace LogicSharp.Services.Models
{
    public class ResultSet
    {
        public  ResultSet(string name, object value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
