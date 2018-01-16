using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Design settings class.
    /// </remarks>
    class DesignerSettings
    {
        /// <value>
        /// Minimal length of the gene-specific part of a primer.
        /// </value>
        public int MinGeneSpecificLen { get; set; }

        /// <value>
        /// Maximal length of the gene-specific part of a primer.
        /// </value>
        public int MaxGeneSpecificLen { get; set; }

        /// <value>
        /// Minimal length of the overlapping part of a primer.
        /// </value>
        public int MinOverlapLen { get; set; }

        /// <value>
        /// Maximal length of the overlapping part of a primer.
        /// </value>
        public int MaxOverlapLen { get; set; }

        /// <value>
        /// CPEC/Gibson assembly reaction volume.
        /// </value>
        public int ReactionVolume { get; set; }

        /// <value>
        /// Target overlaps melting temperature.
        /// </value>
        public int TargetOverlapTm { get; set; }

        /// <value>
        /// Target primer melting temperature.
        /// </value>
        public int TargetPrimerTm { get; set; }

        /// <summary>
        /// Designer settings constructor.
        /// </summary>
        public DesignerSettings()
        {
            this.MinGeneSpecificLen = 18;
            this.MaxGeneSpecificLen = 25;
            this.MinOverlapLen = 20;
            this.MaxOverlapLen = 30;
            this.TargetOverlapTm = 60;
            this.TargetPrimerTm = 60;
            this.ReactionVolume = 50;
        }

    }
}
