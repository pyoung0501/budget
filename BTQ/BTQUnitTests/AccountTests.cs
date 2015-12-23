using BTQ;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BTQUnitTests
{
    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void creating_an_account_without_id_creates_the_account_without_id()
        {
            // Arrange

            // Act
            Account account = new Account();

            // Assert
            Assert.AreEqual("", account.ID);
        }

        [TestMethod]
        public void creating_an_account_with_id_creates_account_with_id()
        {
            // Arrange

            // Act
            Account account = new Account("123");

            // Assert
            Assert.AreEqual("123", account.ID);
        }

        // Assign name
        [TestMethod]
        public void setting_name_on_account_sets_name()
        {
            // Arrange
            Account account = new Account("123");

            // Act
            account.Name = "Account Name";

            // Assert
            Assert.AreEqual("Account Name", account.Name);
        }

        // Assign company
        [TestMethod]
        public void setting_company_on_account_sets_company()
        {
            // Arrange
            Account account = new Account("123");

            // Act
            account.Institution = "Account Institution";

            // Assert
            Assert.AreEqual("Account Institution", account.Institution);
        }
    }
}
