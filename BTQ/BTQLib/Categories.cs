using System.Collections.Generic;

namespace BTQLib
{
    /// <summary>
    /// Defines the categories which can be assigned to a transaction.
    /// Transaction categories consist of a primary category and a secondary
    /// category.
    /// </summary>
    public class Categories
    {
        /// <summary>
        /// The list of primary categories.
        /// </summary>
        public List<string> PrimaryCategories { get; set; }

        /// <summary>
        /// The lists of secondary categories corresponding to each primary category.
        /// </summary>
        private List<List<string>> SecondaryCategories { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Categories()
        {
            PrimaryCategories = new List<string>();
            SecondaryCategories = new List<List<string>>();
        }

        /// <summary>
        /// Gets the list of primary categories.
        /// </summary>
        /// <returns>The list of primary categories.</returns>
        public List<string> GetPrimaryCategories()
        {
            return PrimaryCategories;
        }

        /// <summary>
        /// Gets the secondary categories of the given primary category.
        /// </summary>
        /// <param name="primaryCategory">Primary category.</param>
        /// <returns>The secondary categories of the given primary category.</returns>
        public List<string> GetSecondaryCategories(string primaryCategory)
        {
            int index = PrimaryCategories.BinarySearch(primaryCategory);
            if (index >= 0)
            {
                return SecondaryCategories[index];
            }

            return null;
        }

        /// <summary>
        /// Adds the given category as a primary category.  If the given category
        /// already exists as a primary category, nothing will change.
        /// </summary>
        /// <param name="primaryCategory">Category to add as primary.</param>
        public void AddPrimaryCategory(string primaryCategory)
        {
            int index = PrimaryCategories.BinarySearch(primaryCategory);
            if (index >= 0)
            {
                return;
            }

            PrimaryCategories.Insert(~index, primaryCategory);
            SecondaryCategories.Insert(~index, new List<string>());
        }

        /// <summary>
        /// Returns true if the given primary category exists.
        /// </summary>
        /// <param name="primaryCategory">Category to check.</param>
        /// <returns>True if the given primary category exists.</returns>
        public bool PrimaryCategoryExists(string primaryCategory)
        {
            return PrimaryCategories.BinarySearch(primaryCategory) >= 0;
        }

        /// <summary>
        /// Adds the given secondary category to the specified primary category.
        /// If the primary category does not exist, it will be added.
        /// If the secondary category already exists, nothing will be done.
        /// </summary>
        /// <param name="primaryCategory">Primary category to add secondary to.</param>
        /// <param name="secondaryCategory">Secondary category to add.</param>
        public void AddSecondaryCategory(string primaryCategory, string secondaryCategory)
        {
            if (!PrimaryCategoryExists(primaryCategory))
            {
                AddPrimaryCategory(primaryCategory);
            }

            int primaryIndex = PrimaryCategories.BinarySearch(primaryCategory);
            int secondaryIndex = SecondaryCategories[primaryIndex].BinarySearch(secondaryCategory);
            if (secondaryIndex >= 0)
            {
                return;
            }

            SecondaryCategories[primaryIndex].Insert(~secondaryIndex, secondaryCategory);
        }

        /// <summary>
        /// Returns true if the specified secondary category exists in the specified
        /// primary category.
        /// </summary>
        /// <param name="primaryCategory">Primary category.</param>
        /// <param name="secondaryCategory">Secondary category to check.</param>
        /// <returns>True if the secondary category exists in the primary category.</returns>
        public bool SecondaryCategoryExists(string primaryCategory, string secondaryCategory)
        {
            int primaryIndex = PrimaryCategories.BinarySearch(primaryCategory);
            if (primaryIndex >= 0)
            {
                int secondaryIndex = SecondaryCategories[primaryIndex].BinarySearch(secondaryCategory);
                return secondaryIndex >= 0;
            }

            return false;
        }
    }
}
