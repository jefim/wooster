using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wooster.Classes.Actions;

namespace WoosterTests
{
    [TestClass]
    public class WoosterActionTest
    {
        [TestMethod]
        public void MatchesQueryString_returns_true_if_action_always_visible()
        {
            WoosterAction wa = new WoosterAction();
            wa.SearchableName = "fufufu";
            wa.AlwaysVisible = true;
            Assert.AreEqual(true, wa.MatchesQueryString("hello", false));
        }

        [TestMethod]
        public void MatchesQueryString_returns_true_if_searchable_name_contains_query_string_case_ignored()
        {
            WoosterAction wa = new WoosterAction();
            wa.SearchableName = "This is the searchable name";
            Assert.AreEqual(true, wa.MatchesQueryString("SEarchABle", false));
        }

        [TestMethod]
        public void MatchesQueryString_returns_false_if_searchable_name_does_not_contain_query_string()
        {
            WoosterAction wa = new WoosterAction();
            wa.SearchableName = "This is an action";
            Assert.AreEqual(false, wa.MatchesQueryString("SEarchABle", false));
        }

        [TestMethod]
        public void MatchesQueryString_returns_true_if_searchable_name_first_letters_match_case_ignored()
        {
            WoosterAction wa = new WoosterAction();
            wa.SearchableName = "This is an action";
            Assert.AreEqual(true, wa.MatchesQueryString("t I A", true));
        }
    }
}
