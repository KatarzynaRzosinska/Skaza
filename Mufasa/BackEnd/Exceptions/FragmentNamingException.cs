using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Exceptions
{
    /// <remarks>
    /// Exception thrown if a fragment name is invalid.
    /// <see cref="BackEnd.Designer.Designer.cs"/>
    /// </remarks>
    [Serializable]
    class FragmentNamingException : Exception
    {
        /// <summary>
        /// FragmentNamingException constructor.
        /// </summary>
        public FragmentNamingException() : base()
        {

        }

        /// <summary>
        /// FragmentNamingException constructor.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public FragmentNamingException(string message) : base(message)
        {

        }
    }
}
