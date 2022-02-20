using NUnit.Framework;
using LogicSharp;
using LogicSharp.Models;
using System.Collections.Generic;

namespace LogicSharpTest
{
    public class Tests
    {
        readonly LogicalUnit lunit;

        public Tests()
        {
            var x = new Variable("X");
            var y = new Variable("Y");
            var z = new Variable("Z");

            lunit = new LogicalUnit( new Rule(new Predicate("direkter_vorfahre", "max", "susi")),
                                     new Rule(new Predicate("direkter_vorfahre", "susi", "fred")),
                                     new Rule(new Predicate("direkter_vorfahre", "fred", "herbert")),
                                     new Rule(new Predicate("direkter_vorfahre", "max", "alex")),
                                     new Rule(new Predicate("direkter_vorfahre", "biggi", "basti")),
                                     new Rule(new Predicate("direkter_vorfahre", "alex", "lina")),

                                     new Rule(new Predicate("vorfahre", x, y),
                                              new Predicate("direkter_vorfahre", x, y)),

                                     new Rule(new Predicate("vorfahre", x, y),
                                               new ANDRelation(new Predicate("direkter_vorfahre", x, z),
                                                               new Predicate("vorfahre", z, y))));
           }

        [Test]
        public void Testcase1()
        {
            var result = lunit.Query("vorfahre", "max", "susi");
            Assert.IsTrue(result.Result);
        }

        [Test]
        public void Testcase2()
        {
            var result = lunit.Query("vorfahre", "max", new Variable("K"));

            var expectedList = new List<string> { "susi", "alex", "fred", "herbert", "lina" };
            var matchExpected = true;

            Assert.IsTrue(result.Result && result.ResultSets != null && result.ResultSets.Count == expectedList.Count);

            foreach (var expected in expectedList)
            {
                if (result.ResultSets != null && !result.ResultSets.Exists(r => (string)r.Value == expected))
                {
                    matchExpected = false;
                    break;
                }
            }
            Assert.IsTrue(matchExpected);
        }
    }
}