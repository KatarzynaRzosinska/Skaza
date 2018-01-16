using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Exceptions
{
    /// <remarks>
    /// Exception thrown if sequence count in a file is invalid.
    /// <see cref="BackEnd.Designer.Designer.cs"/>
    /// </remarks>
    [Serializable]
    class SequenceCountException : Exception
    {
        /// <summary>
        /// SequenceCountException constructor.
        /// </summary>
        public SequenceCountException() : base()
        {

        }

        /// <summary>
        /// SequenceCountException constructor.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public SequenceCountException(string message)
            : base(message)
        {

        }
    }
}
