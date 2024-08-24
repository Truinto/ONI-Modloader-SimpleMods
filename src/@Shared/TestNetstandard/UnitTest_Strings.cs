using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.StringsNS;

namespace UnitTest
{
    [TestClass]
    public partial class UnitTests
    {
        [TestMethod]
        public void Test_Strings_SplitArgs()
        {
            (string test, string[] solution)[] values = [
                ( "", [] ),
                ( " ", [] ),
                ( "\" ", [" "] ),
                ( "Foo Bar", ["Foo", "Bar"] ),  //Foo Bar
                ( "Foo Bar ", ["Foo", "Bar"] ),  //Foo Bar 
                ( "Foo\" \"Bar", ["Foo Bar"] ), //Foo" "Bar
                ( "\"Foo Bar\"", ["Foo Bar"] ), //"Foo Bar"
                ( "Foo  \"\"  Bar", ["Foo", "", "Bar"] ),  //Foo  ""  Bar
                ( "\"\"Foo Bar\"\"", ["Foo", "Bar"] ),  //""Foo Bar""
                ( "\"\"\"Foo Bar\"\"\"", ["\"Foo Bar\""] ), //"""Foo Bar"""
                ( "\"\"\"\"Foo Bar\"\"\"\"", ["\"Foo", "Bar\""] ),  //""""Foo Bar""""
                ( "\"\"Foo\" \"Bar\"\"", ["Foo Bar"] ),  //""Foo" "Bar""
                ( "\"Foo \"\"\"Bar\"\"", ["Foo \"Bar"] ),   //"Foo """Bar""
                ( "\"\"a\"\"b\"\"c\"\"d\"\"", [ "abcd" ] ), //""a""b""c""d""
                ( "\"\" \"\" \"\" \"\" \"\"", ["", "", "", "", ""] ),   //"" "" "" "" ""
                ( "\" \" \" \" \" \" \" \" \" \"", [" ", " ", " ", " ", " "] ), //" " " " " " " " " "
                ( "\"\"\"\"\"\"\"\"\"\"\"\"", ["\"\"\"\"\""] ), //""""""""""""
            ];

            int i = 0;
            foreach (var (test, solution) in values)
            {
                var actual = Strings.SplitArgs(test);
                CollectionAssert.AreEqual(solution, actual, $"{i++} '{test}'({actual.Count}) != '{Strings.Join(solution)}'({solution.Length})");
            }
        }

        [TestMethod]
        public void Test_Strings_JoinArgs()
        {
            (string[] test, string solution)[] values = [
                ( [], "" ),
                ( [""], "\"\"" ), //""
                ( ["Foo"], "Foo" ),
                ( ["Foo", "Bar"], "Foo Bar" ),
                ( ["Foo Bar"], "\"Foo Bar\"" ),
                ( ["Foo\"Bar"], "\"Foo\"\"Bar\"" ), //"Foo""Bar"
                ( ["\"Foo Bar\""], "\"\"\"Foo Bar\"\"\"" ), //"""Foo Bar"""
                ( ["\"Foo", "Bar\""], "\"\"\"Foo\" \"Bar\"\"\"" ), //"""Foo" "Bar"""
                ( ["Foo\" \"Bar"], "\"Foo\"\" \"\"Bar\"" ), //"Foo"" ""Bar"
                ( ["Foo ", "", " Bar"], "\"Foo \" \"\" \" Bar\"" ),
                ( ["Foo", "", "Bar"], "Foo \"\" Bar" ),
                ( ["Foo \"", "", "\" Bar"], "\"Foo \"\"\" \"\" \"\"\" Bar\"" ), //"Foo """ "" """ Bar"
            ];

            int i = 0;
            foreach (var (test, solution) in values)
            {
                var actual = Strings.JoinArgs(test);
                Assert.AreEqual(solution, actual, $"{i++}");
            }
        }
    }
}
