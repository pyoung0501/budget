using BTQLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace BTQUnitTests
{
    [TestClass]
    public class TransactionTests
    {
        [TestMethod]
        public void setting_payee_sets_payee_field()
        {
            // Arrange
            Transaction transaction = new Transaction();

            // Act
            transaction.Payee = "Payee";

            // Assert
            Assert.AreEqual("Payee", transaction.Payee);
        }

        [TestMethod]
        public void setting_description_sets_description_field()
        {
            // Arrange
            Transaction transaction = new Transaction();

            // Act
            transaction.Description = "Description";

            // Assert
            Assert.AreEqual("Description", transaction.Description);
        }

        [TestMethod]
        public void setting_amount_sets_amount_field()
        {
            // Arrange
            Transaction transaction = new Transaction();

            // Act
            transaction.Amount = 123;

            // Assert
            Assert.IsTrue(123 == transaction.Amount);
        }

        [TestMethod]
        public void setting_category_sets_category_field()
        {
            // Arrange
            Transaction transaction = new Transaction();

            // Act
            transaction.Category = "Category:SubCategory";

            // Assert
            Assert.AreEqual("Category:SubCategory", transaction.Category);
        }

        [TestMethod]
        public void setting_date_sets_date_field()
        {
            // Arrange
            Transaction transaction = new Transaction();
            DateTime dateTime = new System.DateTime(2015, 3, 5);
            DateTime dateTime2 = new System.DateTime(2015, 3, 5);

            // Act
            transaction.Date = dateTime;

            // Assert
            Assert.AreEqual(dateTime2, transaction.Date);
        }
    }
}
