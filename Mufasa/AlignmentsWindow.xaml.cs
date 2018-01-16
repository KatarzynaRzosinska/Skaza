using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bio;
using Bio.Algorithms;

using Bio.IO.GenBank;
using Bio.Algorithms.Alignment;
using Bio.SimilarityMatrices;


namespace Mufasa
{
    /// <summary>
    /// Interaction logic for AlignmentsWindow.xaml
    /// </summary>
    public partial class AlignmentsWindow : ModernWindow
    {
        public AlignmentsWindow()
        {
            InitializeComponent();
            InitializeAlignment();
        }

        private static string seqq1="AAACGCGTACGATGCCCGAGTATTAGGACCATACCGACTCAGCTATATATCGCTATACTAGGGATATCCCATATCGCGATTATCTCAGCTACT";
        private static string seqq2 = "AAACGCGTACTATGCCCGACTATTAGGACCATACCGACTCAGCTAAATCGACCTATACTAGGGATATCCCATATCGCGATTATCTCAGCTACT";

        public ISequence seq1 = new Sequence(Alphabets.DNA, seqq1);
        public ISequence seq2 = new Sequence(Alphabets.DNA, seqq2);

        private void  InitializeAlignment()
        {
            DnaAlphabet dna = DnaAlphabet.Instance;

        }
    }
}
