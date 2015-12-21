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
    }
}
