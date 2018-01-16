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
using Bio.IO;

namespace Mufasa
{
    /// <summary>
    /// Interaction logic for ModernWindow1.xaml
    /// </summary>
    public partial class ModernWindow1 : ModernWindow 
    {
        public ModernWindow1(string seq)
        {
            InitializeComponent();
            this.sequence = seq;
            InitializeSelectSequenceTextBox();
            
            
        }

        private string sequence { get; set; }
        public ISequence selectSequence { get; set; } 
        private Microsoft.Win32.SaveFileDialog saveEditSequenceFileDialog;



        private void InitializeSelectSequenceTextBox()
        {
            selectSequenceTextBox.Text = sequence;
        }
        private void reverse_Click(object sender, RoutedEventArgs e)
        {
            this.selectSequence = new Sequence(Alphabets.DNA, sequence);
            changeTextBox.Text = GetReverseString(selectSequence);
        }

        private void compl_Click(object sender, RoutedEventArgs e)
        {
            this.selectSequence = new Sequence(Alphabets.DNA, sequence);
            changeTextBox.Text = GetComplementString(selectSequence);
        }

        private void revCom_Click(object sender, RoutedEventArgs e)
        {
            this.selectSequence = new Sequence(Alphabets.DNA, sequence);
            changeTextBox.Text = GetReverseComplementString(selectSequence);
        }


        private void InitializeSaveSequenceFileDialog()
        {
            this.saveEditSequenceFileDialog = new Microsoft.Win32.SaveFileDialog();
            this.saveEditSequenceFileDialog.FileName = ""; // Default file name
            this.saveEditSequenceFileDialog.DefaultExt = ".gb"; // Default file extension
            this.saveEditSequenceFileDialog.Filter = "GenBank files| *.gb;*.gbk|Fasta files|*.fa;*.fas;*.fasta|All files|*.*"; // Filter files by extension

            this.saveEditSequenceFileDialog.Title = "Save sequence...";
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
          if (changeTextBox.Text !=null)
           {
               InitializeSaveSequenceFileDialog();

               if (saveEditSequenceFileDialog.ShowDialog() == true)
                   try
                   {
                    string sequenceToSave = changeTextBox.Text;
                    ISequence seq = new Bio.Sequence(Alphabets.DNA, sequenceToSave);
                    ISequenceFormatter formatter = SequenceFormatters.FindFormatterByFileName(saveEditSequenceFileDialog.FileName);
                    formatter.Write(seq);
                    formatter.Close();
                   }
                   catch (Exception ex)
                   {
                       MessageBoxResult result = ModernDialog.ShowMessage(ex.Message, "Exception", MessageBoxButton.OK);
                   }
           }

        }


        private string GetString(ISequence seq)
        {
            char[] symbols = new char[seq.Count];
            for (long index = 0; index < seq.Count; index++)
            {
                symbols[index] = (char)seq[index];
            }

            return new String(symbols);
        }

        private string GetReverseComplementString(ISequence seq)
        {
            ISequence revComp = seq.GetReverseComplementedSequence();
            char[] symbols = new char[revComp.Count];
            for (long index = 0; index < revComp.Count; index++)
            {
                symbols[index] = (char)revComp[index];
            }
            return new String(symbols);
        }
        private string GetComplementString(ISequence seq)
        {
            ISequence Comp = seq.GetComplementedSequence();
            char[] symbols = new char[Comp.Count];
            for (long index = 0; index < Comp.Count; index++)
            {
                symbols[index] = (char)Comp[index];
            }
            return new String(symbols);
        }

        private string GetReverseString(ISequence seq)
        {
            ISequence Comp = seq.GetReversedSequence();
            char[] symbols = new char[Comp.Count];
            for (long index = 0; index < Comp.Count; index++)
            {
                symbols[index] = (char)Comp[index];
            }
            return new String(symbols);
        }


    }
}
