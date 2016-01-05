using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wooster.ActionProviders;
using Wooster.Classes.Actions;
using Wooster.Classes;

namespace WoosterTests
{
    [TestClass]
    public class LocalProgramsActionProviderTest
    {
        [TestMethod]
        public void MatchesQueryString_returns_true_if_action_always_visible()
        {
            var provider = new LocalProgramsActionProvider();
            WoosterAction wa = new WoosterAction();
            wa.SearchableName = "fufufu";
            wa.AlwaysVisible = true;
            Assert.AreEqual(true, provider.MatchesQueryString("hello", wa));
        }

        [TestMethod]
        public void MatchesQueryString_returns_true_if_searchable_name_contains_query_string_case_ignored()
        {
            var provider = new LocalProgramsActionProvider();
            WoosterAction wa = new WoosterAction();
            wa.SearchableName = "This is the searchable name";
            Assert.AreEqual(true, provider.MatchesQueryString("SEarchABle", wa));
        }

        [TestMethod]
        public void MatchesQueryString_returns_false_if_searchable_name_does_not_contain_query_string()
        {
            var provider = new LocalProgramsActionProvider();
            WoosterAction wa = new WoosterAction();
            wa.SearchableName = "This is an action";
            Assert.AreEqual(false, provider.MatchesQueryString("SEarchABle", wa));
        }

        [TestMethod]
        public void MatchesQueryString_returns_true_if_searchable_name_first_letters_match_case_ignored()
        {
            var provider = new LocalProgramsActionProvider();
            provider.Initialize(new Config() { SearchByFirstLettersEnabled = true });
            WoosterAction wa = new WoosterAction();
            wa.SearchableName = "This is an action";
            Assert.AreEqual(true, provider.MatchesQueryString("t I A", wa));
        }
    }
}
