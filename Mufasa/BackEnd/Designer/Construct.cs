using Bio;
using Bio.IO;
using Bio.IO.GenBank;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Genetic construct class.
    /// </remarks>
    class Construct : Fragment
    {

        /// <summary>
        /// Construct constructor.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        public Construct(ObservableCollection<Fragment> fragList, DesignerSettings settings)
            : base()
        {
            Init(fragList, settings);
        }

        /// <summary>
        /// Construct constructor.
        /// </summary>
        /// <param name="fragDict">Fragment Dictionary.</param>
        /// <param name="nameList">Fragment names. Dictionary keys.</param>
        public Construct(ObservableCollection<String> nameList, Dictionary<String, Fragment> fragDict, DesignerSettings settings)
            : base()
        {
            ObservableCollection<Fragment > fragList = new ObservableCollection<Fragment>();
            for (int i = 0; i < nameList.Count; i++ )
            {
                Fragment f = fragDict[nameList[i]];
                if (i==0)
                {
                    f.IsVector = true;
                }
                fragList.Add(f);
            }
            Init(fragList, settings);
        }

        /// <value>
        /// Designer settings.
        /// </value>
        public DesignerSettings Settings { get; set; }

        /// <summary>
        /// Construct initialization.
        /// </summary>
        /// <param name="fragList">Fragment list.</param>
        /// <param name="maxOverlapLen">Minimum overlap length.</param>
        private void Init(ObservableCollection<Fragment> fragList, DesignerSettings settings)
        {
            this.Overlaps = new List<Overlap>();
            this.Settings = settings;
            //forward
            String seq5 = "";
            String seq3 = "";
            String name = "";
            List<MiscFeature> featList = new List<MiscFeature>();
            for (int i = 0; i < fragList.Count; i++)
            {
                name += fragList[i].Name;
                seq3 = fragList[i].GetString();
                int len5 = Math.Min(settings.MaxOverlapLen, seq5.Length);
                int len3 = Math.Min(settings.MaxGeneSpecificLen, seq3.Length);
                String overlapping = seq5.Substring(seq5.Length - len5, len5);
                String geneSpecific = seq3.Substring(0, len3);
                String loc = (seq5.Length + 1).ToString() + ".." + (seq5.Length + seq3.Length).ToString();
                MiscFeature gene = new MiscFeature(loc);
                gene.StandardName = fragList[i].Name;
                featList.Add(gene);
                seq5 += seq3;
                if (i == 0)
                {
                    Overlaps.Add(new Overlap(fragList[i].Name + "_fwd", new Sequence(Alphabets.DNA, geneSpecific)));
                }
                else
                {

                    Overlaps.Add(new Overlap(fragList[i].Name + "_fwd", new Sequence(Alphabets.DNA, overlapping), new Sequence(Alphabets.DNA, geneSpecific)));
                }                 
            }

            this.Sequence = new Sequence(Alphabets.DNA, seq5);
            //meta
            GenBankMetadata meta = new GenBankMetadata();
            meta.Locus = new GenBankLocusInfo();
            meta.Locus.MoleculeType = MoleculeType.DNA;
            meta.Locus.Name = name;
            meta.Locus.Date = System.DateTime.Now;
            meta.Locus.SequenceLength = seq5.Length;
            meta.Comments.Add("designed with mufasa");
            meta.Definition = "synthetic construct";
            meta.Features = new SequenceFeatures();
            meta.Features.All.AddRange(featList);
            this.Sequence.Metadata.Add("GenBank", meta);

            //reverse
            fragList.Add(new Fragment(fragList[0]));
            fragList.RemoveAt(0);
            seq5 = "";
            seq3 = "";
            for (int i = fragList.Count-1; i >= 0; i--)
            {
                seq5 = fragList[i].GetReverseComplementString();
                int len3 = Math.Min(settings.MaxOverlapLen, seq3.Length);
                int len5 = Math.Min(settings.MaxGeneSpecificLen, seq5.Length);
                String overlapping = seq3.Substring(seq3.Length - len3, len3);
                String geneSpecific = seq5.Substring(0, len5);
                seq3 += seq5;
                if (i == fragList.Count - 1)
                {
                    Overlaps.Add(new Overlap(fragList[i].Name + "_rev", new Sequence(Alphabets.DNA, geneSpecific)));
                }
                else
                {

                    Overlaps.Add(new Overlap(fragList[i].Name + "_rev", new Sequence(Alphabets.DNA, overlapping), new Sequence(Alphabets.DNA, geneSpecific)));
                }
            }
            TermoOptimizeOverlaps();
        }

        /// <value>
        /// Generated overlaps collection.
        /// </value>
        public List<Overlap> Overlaps { get; set; }


        /// <summary>
        /// Save in one of .NET Bio supported formats like fasta or GenBank.
        /// </summary>
        /// <param name="path">Filename.</param>
        public void SaveAsBio(String path)
        {
            ISequenceFormatter formatter = SequenceFormatters.FindFormatterByFileName(path);
            formatter.Write(this.Sequence);
            formatter.Close();
        }

        /// <summary>
        /// Overlap temperature optimization.
        /// </summary>
        private void TermoOptimizeOverlaps()
        {
            byte end = 255;
            for (int i = 0; i < this.Overlaps.Count; i++)
            {
                byte item = 0;
                bool tmTooHigh = (this.Overlaps[i].Temperature > this.Settings.TargetOverlapTm);
                while ((item != end) && tmTooHigh)
                {
                    item = this.Overlaps[i].Dequeue(Settings.MinOverlapLen);
                    tmTooHigh = (this.Overlaps[i].Temperature > this.Settings.TargetOverlapTm);
                }

                item = 0;
                tmTooHigh = (this.Overlaps[i].PrimerTemperature > this.Settings.TargetPrimerTm);
                while ((item != end) && tmTooHigh)
                {
                    // not vector primers
                    item = this.Overlaps[i].Pop(Settings.MinGeneSpecificLen);
                    tmTooHigh = (this.Overlaps[i].PrimerTemperature > this.Settings.TargetPrimerTm);
                }
            }
        }
    }
}
