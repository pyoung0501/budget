using BTQLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BTQUnitTests
{
    [TestClass]
    public class ProfileTests
    {
        [TestMethod]
        public void creating_a_profile_creates_a_profile_with_no_name()
        {
            // Arrange

            // Act
            Profile profile = new Profile();

            // Assert
            Assert.AreEqual("", profile.Name);
        }

        [TestMethod]
        public void setting_name_on_a_profile_changes_its_name()
        {
            // Arrange
            Profile profile = new Profile();
            string NameToSetOnProfile = "Fred";

            // Act
            profile.Name = NameToSetOnProfile;

            // Assert
            Assert.AreEqual(NameToSetOnProfile, profile.Name);
        }

        [TestMethod]
        public void getting_name_on_a_profile_returns_its_name()
        {
            // Arrange
            string InitialProfileName = "Fred";
            Profile profile = new Profile() { Name = InitialProfileName };

            // Act
            string profileName = profile.Name;

            // Assert
            Assert.AreEqual(InitialProfileName, profileName);
        }

        [TestMethod]
        public void creating_an_account_increases_number_of_accounts_by_one()
        {
            // Arrange
            Profile profile = new Profile();

            // Act
            profile.CreateAccount("123");

            // Assert
            Assert.AreEqual(1, profile.Accounts.Count);
        }

        [TestMethod]
        public void removing_an_account_by_name_decreases_number_of_accounts_by_one()
        {
            // Arrange
            Profile profile = new Profile();
            profile.CreateAccount("123");

            // Act
            profile.RemoveAccount("123");

            // Assert
            Assert.AreEqual(0, profile.Accounts.Count);
        }

        [TestMethod]
        public void creating_an_account_with_a_name_matching_an_existing_one_does_not_increase_count()
        {
            // Arrange
            Profile profile = new Profile();
            profile.CreateAccount("123");

            // Act
            profile.CreateAccount("123");

            // Assert
            Assert.AreEqual(1, profile.Accounts.Count);
        }

        [TestMethod]
        public void getting_an_account_by_name_not_in_profile_returns_null()
        {
            // Arrange
            Profile profile = new Profile();

            // Act
            Account account = profile.GetAccount("123");

            // Assert
            Assert.AreEqual(null, account);
        }

        [TestMethod]
        public void getting_an_account_by_name_existing_in_profile_returns_account()
        {
            // Arrange
            Profile profile = new Profile();
            profile.CreateAccount("123");

            // Act
            Account account = profile.GetAccount("123");

            // Assert
            Assert.AreNotEqual(null, account);
            Assert.AreEqual("123", account.Name);
        }
    }
}
