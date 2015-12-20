using BTQ;
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
    }
}
