using Bio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Exceptions
{
    /// <remarks>
    /// Exception thrown if sequence length in a file is invalid.
    /// <see cref="BackEnd.Designer.Designer.cs"/>
    /// </remarks>
    [Serializable]
    class SequenceLengthException : Exception
    {
        /// <summary>
        /// Sequence in question.
        /// </summary>
        public ISequence Sequence { get; set; }

        /// <summary>
        /// SequenceLengthException constructor.
        /// </summary>
        public SequenceLengthException()
            : base()
        {

        }

        /// <summary>
        /// SequenceLengthException constructor.
        /// </summary>
        /// <param name="message">Message to send.</param>
        public SequenceLengthException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// SequenceLengthException constructor.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="sequence">Sequence in question.</param>
        public SequenceLengthException(string message, ISequence sequence)
            : base(message)
        {
            this.Sequence = sequence;
        }
    }
}
