using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTQ
{
    /// <summary>
    /// Profile associated with a user.  A profile will contain or be
    /// linked to accounts.
    /// </summary>
    public class Profile
    {
        /// <summary>
        /// Name of the profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructs a new instance of Profile.
        /// </summary>
        public Profile()
        {
            Name = "";
        }
    }
}
