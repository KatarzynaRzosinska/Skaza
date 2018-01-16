using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bio;
using Bio.IO;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Overlap class.
    /// </remarks>
    class Overlap : Fragment
    {
        /// <summary>
        /// Overlap constructor.
        /// </summary>
        /// <param name="name">Overlap name.</param>
        /// <param name="overlapping">Overlap sequence.</param>
        /// <param name="geneSpecific">Gene specific sequence.</param>
        public Overlap(String name, ISequence overlapping, ISequence geneSpecific)
        {
            this.GeneSpecific = geneSpecific;
            this.Overlapping = overlapping;
            this.Name = name;
            this.Sequence = new Sequence(Alphabets.DNA, overlapping.ToString() + geneSpecific.ToString());
            this.TempInit();
        }

        /// <summary>
        /// Overlap constructor.
        /// </summary>
        /// <param name="name">Overlap name.</param>
        /// <param name="primer">Primer sequence.</param>
        public Overlap(String name, ISequence primer)
        {
            this.GeneSpecific = primer;
            this.Overlapping = new Sequence(Alphabets.DNA, "");
            this.Name = name;
            this.Sequence = new Sequence(Alphabets.DNA, primer.ToString());
            this.TempInit();
        }

        /// <summary>
        /// Simple temperature computation initialization.
        /// </summary>
        private void TempInit()
        {
            this.SimpleT = new Dictionary<byte, int>();
            this.SimpleT.Add(Alphabets.DNA.A, 2);
            this.SimpleT.Add(Alphabets.DNA.T, 2);
            this.SimpleT.Add(Alphabets.DNA.G, 4);
            this.SimpleT.Add(Alphabets.DNA.C, 4);
            this.SimpleT.Add(Alphabets.DNA.Gap, 0);
            this.Temperature = GetSimpleMeltingTemperature(Overlapping);
            this.PrimerTemperature = GetSimpleMeltingTemperature(GeneSpecific);
        }


        /// <value>
        /// Nucleotide temperature dictionary.
        /// </value>
        private Dictionary <byte,int> SimpleT;

        /// <value>
        /// Overlap's temperature.
        /// </value>
        public int Temperature { get; set; }

        /// <value>
        /// Primer's temperature.
        /// </value>
        public int PrimerTemperature { get; set; }

        /// <value>
        /// Gene specific subsequence.
        /// </value>
        public ISequence GeneSpecific { get; set; }

        /// <value>
        /// Overlapping subsequence.
        /// </value>
        public ISequence Overlapping { get; set; }

        /// <summary>
        /// Prints the overlap in a human-readable format.
        /// </summary>
        /// <returns>String represanting the overlap.</returns>
        public override string ToString()
        {
            String sep = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            String result = this.Name + sep + this.Sequence + sep + this.Temperature + sep + this.PrimerTemperature;
            return result;
        }

        /// <value>
        /// Sequence string.
        /// </value>
        public String SequenceString
        {
            get { return this.Sequence.ToString(); }
        }


        /// <summary>
        /// Compute overlap's simple-style melting temperature.
        /// </summary>
        /// <returns>Overlap's Tm.</returns>
        public int GetSimpleMeltingTemperature(ISequence sequence)
        {
            int T = 0;
            Sequence upper = null;
            upper = new Sequence(Alphabets.DNA, sequence.ToString().ToUpper());            
            
            for (long index = 0; index < upper.Count; index++)
            {
                T += SimpleT[upper[index]];
            }
            return T;
        }

        /// <summary>
        /// Cut the first nucleotide off.
        /// </summary>
        /// <param name="minLen">Minimum overlap length.</param>
        /// <returns>First nucleotide or 255 if oligo too short to dequeue.</returns>
        public byte Dequeue(int minLen)
        {
            if (IsDequeAllowed(minLen))
            {
                byte item = this.Overlapping[this.Overlapping.Count - 1];
                this.Overlapping = this.Overlapping.GetSubSequence(1, this.Overlapping.Count - 1);
                this.Sequence = new Sequence(Alphabets.DNA, Overlapping.ToString() + GeneSpecific.ToString());
                this.Temperature = GetSimpleMeltingTemperature(Overlapping);
                return item;
            }
            else
            {
                return 255;
            }
        }

        /// <summary>
        /// Cut the last nucleotide of.
        /// </summary>
        /// <param name="minLen">Minimum primer length.</param>
        /// <returns>Last nucleotide or 255 if oligo too short to pop.</returns>
        public byte Pop(int minLen)
        {
            if (IsPopAllowed(minLen))
            {
                byte item = this.GeneSpecific[0];
                this.GeneSpecific = this.GeneSpecific.GetSubSequence(0, this.GeneSpecific.Count - 1);
                this.Sequence = new Sequence(Alphabets.DNA, Overlapping.ToString() + GeneSpecific.ToString());
                this.PrimerTemperature = GetSimpleMeltingTemperature(GeneSpecific);
                return item;
            }
            else
            {
                return 255;
            }
        }

        /// <summary>
        /// Check if pop is allowed.
        /// </summary>
        /// <returns>True if conditions satisfied.</returns>
        private bool IsPopAllowed(int minLen)
        {
            //To modify.
            if (this.GeneSpecific.Count > minLen )
            {
                return true;
            }
            else
            {
                return false;
            }
                
        }

        /// <summary>
        /// Check if deque is allowed.
        /// </summary>
        /// <returns>True if conditions satisfied.</returns>
        private bool IsDequeAllowed(int minLen)
        {
            //To modify.
            if (this.Overlapping.Count > minLen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
