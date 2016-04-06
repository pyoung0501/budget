using BTQLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BTQUnitTests
{
    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void creating_an_account_without_name_creates_the_account_without_name()
        {
            // Arrange

            // Act
            Account account = new Account();

            // Assert
            Assert.AreEqual("", account.Name);
        }

        [TestMethod]
        public void creating_an_account_with_name_creates_account_with_name()
        {
            // Arrange

            // Act
            Account account = new Account("123");

            // Assert
            Assert.AreEqual("123", account.Name);
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

        [TestMethod]
        public void adding_transaction_to_account_increases_number_of_transactions_by_one()
        {
            // Arrange
            Account account = new Account("123");

            // Act
            account.AddTransaction(new Transaction());

            // Assert
            Assert.AreEqual(1, account.Transactions.Count);
        }
    }
}
