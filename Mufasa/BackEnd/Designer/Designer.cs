using Mufasa.BackEnd.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bio.IO;
using Bio;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows;
using System.Net;
using Bio.IO.GenBank;

namespace Mufasa.BackEnd.Designer
{
    /// <remarks>
    /// Construct designer class.
    /// </remarks>
    class Designer
    {
        /// <summary>
        /// Designer constructor.
        /// </summary>
        public Designer()
        {
            this.FragmentDict = new Dictionary<String, Fragment>();
            this.ConstructionList = new ObservableCollection<String>();
            this.Settings = new DesignerSettings();
        }

        /// <value>
        /// Fragment file parser
        /// </value>
        private ISequenceParser parser;

        /// <value>
        /// Dictionary of pooled fragments.
        /// </value>
        public Dictionary<String,Fragment> FragmentDict { get; set; }

        /// <value>
        /// Dictionary of construction fragments.
        /// </value>
        public ObservableCollection<String> ConstructionList { get; set; }

        /// <value>
        /// Designer settings.
        /// </value>
        public DesignerSettings Settings { get; set; }

        /// <summary>
        /// Adds Fragment <paramref name="name"/> if valid.
        /// </summary>
        /// <param name="file">Fragment filename</param>
        /// <param name="name">Fragment name</param>
        public void AddFragment(String file, String name)
        {
            if (this.FragmentDict.Keys.Contains(name))
            {
                throw new FragmentNamingException(name);
            }
            ISequence sequence = null;
            try
            {
                sequence = ParseFile(file);
                this.FragmentDict.Add(name, new Fragment(file, name, sequence));
            }
            catch(SequenceLengthException sle)
            {
               SequenceTooShort(file, name, sle);
            }           
            
        }

        /// <summary>
        /// Asks for confirmation of a too short sequence use.
        /// </summary>
        /// <param name="name">Fragment name.</param>
        /// <param name="source">File path or url.</param>
        /// <param name="sle">Exception to handle.</param>
        /// <returns></returns>
        private void SequenceTooShort(String source, String name, SequenceLengthException sle)
        {
            MessageBoxResult result = ModernDialog.ShowMessage(sle.Message + Environment.NewLine + "Do you really want to use it?", "warning: " + name, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.FragmentDict.Add(name, new Fragment(source, name, sle.Sequence));
            }
        }


        /// <summary>
        /// Parses a file containing a sequence in .NET Bio supported formats.
        /// </summary>
        /// <param name="file">Path to a parsed file.</param>
        /// <returns>Fragment sequence.</returns>
        private ISequence ParseFile(String file)
        {
            parser = SequenceParsers.FindParserByFileName(file);
            if (parser == null)
            {
                throw new FileFormatException("Parser for " + file + " not found.");
            }
            return ParseFragment(parser, file);
        }

        /// <summary>
        /// Parses a fragment using an appropriate parser.
        /// </summary>
        /// <param name="parser">Open parser.</param>
        /// <param name="fragmentName">Fragment name.</param>
        /// <returns>Fragment sequence.</returns>
        private ISequence ParseFragment(ISequenceParser parser, String fragmentName)
        {
            List<ISequence> sequences;
            using (parser)
            {
                sequences = parser.Parse().ToList();
            }
            if (sequences.Count != 1)
            {
                throw new SequenceCountException(fragmentName + " contains " + sequences.Count + " well-formatted sequences. It should contain exactly one.");
            }
            if (sequences.First().Count < 150)
            {
                throw new SequenceLengthException("Sequence in " + fragmentName + " is shorter than 150nt. It should not be used as a fragment.", sequences.First());
            }
            return sequences.First();
        }

        /// <summary>
        /// Adds a BioBrick <paramref name="name"/> if valid.
        /// </summary>
        /// <param name="url">URL to a BioBrick in .fasta format.</param>
        /// <param name="name">BioBrock name.</param>
        public void AddBrickFromRegistry(String url, String sequenceString, String name)
        {
            sequenceString = sequenceString.Replace(" ", "");
            sequenceString = sequenceString.Replace("\n", "");
            if (this.FragmentDict.Keys.Contains(name))
            {
                throw new FragmentNamingException(name);
            }

            if (sequenceString.Length < 150)
            {
                SequenceTooShort(url, name, new SequenceLengthException("Sequence in " + name + " is shorter than 150nt. It should not be used as a fragment.", new Sequence(Alphabets.DNA, sequenceString)));
            }
            else
            {
                this.FragmentDict.Add(name, new Fragment(url, name, new Sequence(Alphabets.DNA, sequenceString)));
            }
        } 
        
        /// <summary>
        /// Adds a fragment to construction dictionary.
        /// </summary>
        /// <param name="fragment">Name of the fragment to add.</param>
        public void AddConstructionFragment(String fragmentName)
        {
            if (this.ConstructionList.Contains(fragmentName))
            {
                throw new FragmentNamingException(fragmentName);
            }

            this.ConstructionList.Add(fragmentName);
        }

        public void openProject(String file)
        {
            Sequence sequence = null;

            parser = SequenceParsers.GenBank;
            parser.Open(file);
            sequence = (Sequence)parser.Parse().ToList()[0];
            parser.Close();

            Fragment project = new Fragment(file, "project", sequence);
            GenBankMetadata meta = sequence.Metadata["GenBank"] as GenBankMetadata;
            FragmentDict = new Dictionary<string, Fragment>();
            foreach (var feat in meta.Features.MiscFeatures)
            {
                String subseq = project.GetString().Substring(feat.Location.LocationStart-1, feat.Location.LocationEnd - feat.Location.LocationStart + 1);
                FragmentDict.Add(feat.StandardName, new Fragment(file, feat.StandardName, new Sequence(Alphabets.DNA, subseq)));
            }
        }        
    }
}


