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
using System.Collections.ObjectModel;
using Mufasa.BackEnd.Designer;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Bio;
using Bio.Algorithms;
using Microsoft.Win32;
using System.IO;
using FirstFloor.ModernUI.Windows.Controls;
using Bio.IO;




namespace Mufasa.Pages
{
    /// <summary>
    /// Interaction logic for Sequence.xaml
    /// </summary>
    public partial class Sequence : UserControl
    {

        private static string[] CODONS = { 
                "TTT", "TTC", "TTA", "TTG", "TCT",
                "TCC", "TCA", "TCG", "TAT", "TAC", "TGT", "TGC", "TGG", "CTT",
                "CTC", "CTA", "CTG", "CCT", "CCC", "CCA", "CCG", "CAT", "CAC",
                "CAA", "CAG", "CGT", "CGC", "CGA", "CGG", "ATT", "ATC", "ATA",
                "ATG", "ACT", "ACC", "ACA", "ACG", "AAT", "AAC", "AAA", "AAG",
                "AGT", "AGC", "AGA", "AGG", "GTT", "GTC", "GTA", "GTG", "GCT",
                "GCC", "GCA", "GCG", "GAT", "GAC", "GAA", "GAG", "GGT", "GGC",
                "GGA", "GGG", "TGA", "TAG", "TAA" };

        private static string[] AMINOS_PER_CODON = { 
            "F", "F", "L", "L", "S", "S",
            "S", "S", "Y", "Y", "C", "C", "W", "L", "L", "L", "L", "P", "P",
            "P", "P", "H", "H", "Q", "Q", "R", "R", "R", "R", "I", "I", "I",
            "M", "T", "T", "T", "T", "N", "N", "K", "K", "S", "S", "R", "R",
            "V", "V", "V", "V", "A", "A", "A", "A", "D", "D", "E", "E", "G",
            "G", "G", "G", "*", "*", "*" };

        private static string[] AMINOS_PER_CODON_FULL = {
            "Phe", "Phe", "Leu", "Leu", "Ser", "Ser",
            "Ser", "Ser", "Tyr", "Tyr", "Cys", "Cys", "Trp", "Leu", "Leu", "Leu", "Leu", "Pro", "Pro",
            "Pro", "Pro", "His", "His", "Gln", "Gln", "Arg", "Arg", "Arg", "Arg", "Ile", "Ile", "Ile",
            "Met", "Thr", "Thr", "Thr", "Thr", "Asn", "Asn", "Lys", "Lys", "Ser", "Ser", "Arg", "Arg",
            "Val", "Val", "Val", "Val", "Ala", "Ala", "Ala", "Ala", "Asp", "Asp", "Glu", "Glu", "Gly",
            "Gly", "Gly", "Gly", "*S*", "*S*", "*S*" };

        private static string[] FULL_NAMES = {
            "phenylalanine", "leucine",
            "isoleucine", "methionine", "valine", "serine", "proline",
            "threonine", "alanine", "tyrosine", "histidine", "glutamine",
            "asparagine", "lysine", "aspartic acid", "glutamic acid",
            "cysteine", "tryptophan", "arginine", "glycine" };

        public Boolean aminoFullNames { get; set; }
        private Microsoft.Win32.SaveFileDialog saveSequenceFileDialog;

        public Sequence()
        {

            doNotUpdate = true;

            InitializeComponent();
            Circle = new Circle(circleCanvas);
            

            if (Design.Designer.FragmentDict.Keys.Count() != 0)
            {
                InitializeSequenceListView();
            }

            aminoCode = false;
            if (aminoCode)
                DNAlineInBlock = 2;
            else DNAlineInBlock = 1;

            seqMultiLine = 4; //ile linii blokowych
            blockSize = 9; //ile liter w bloku danych
            blocksInLineCounter = 10; //ile bloków danych w linii
            blockLineTotalLength = (blockSize * blocksInLineCounter) + (blocksInLineCounter - 1);

            //przykład strzałek:

            //arrowsData.Add(new ArrowBox(130, 500, 1000, 2000, Brushes.Red));
 
            //arrowsData.Add(new ArrowBox(110, 1900, 1800, 2000, Brushes.OrangeRed));

            sequence = sequence + sequence;
            

            //rawFASTAtextBox.Text = sequence;
            //sequenceTextBox.Text = createProDNA(sequence, aminoCode);
            textSize = sequenceTextBox.Text.Length;
            doNotUpdate = false;
        }

        internal static Circle Circle { get; set; }
        public Fragment select { get; set; }

        private ObservableCollection<Fragment> sequenceList;

        private int seqMultiLine; //ile linii blokowych
        private int blockSize; //ile liter w bloku danych
        private int blocksInLineCounter; //ile bloków danych w linii
        private int blockLineTotalLength;
        private Boolean doNotUpdate = false; //wyłącza metode wyskakujaca w czasie edycji czegokolwiek w TextBox
        private int textSize = 0;

        public Boolean aminoCode { get; set; }
        

        private int DNAlineInBlock = 1; //która linia wieloliniowego bloku to DNA

        private List<ArrowBox> arrowsData = new List<ArrowBox>();
        private int caretStart;

        private string sequence;
        string selectedDNAs;
        public string sequenceName;
     

       private void InitializeSequenceListView()
       {
           
           sequenceList = new ObservableCollection<Fragment>();
            
           foreach (string s in Design.Designer.FragmentDict.Keys)
           {

               Fragment f= Design.Designer.FragmentDict[s];
               sequenceList.Add(f);
               
               
           }
           this.Elements = new ObservableCollection<FragmentViewModel>(sequenceList.Select(m => new FragmentViewModel(m)));
           this.Sequences = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Elements);
           this.Elements.CollectionChanged += new NotifyCollectionChangedEventHandler(Elements_CollectionChanged);
           foreach (var m in this.Elements)
               m.PropertyChanged += new PropertyChangedEventHandler(Element_PropertyChanged);

           sequenceListView.ItemsSource = Sequences;
       }

       
       void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
       {
           //Update the collection view if refresh isn't possible
           if (this.Sequences.IsEditingItem)
               this.Sequences.CommitEdit();
           if (this.Sequences.IsAddingNew)
               this.Sequences.CommitNew();

           this.Sequences.Refresh();
           for (int i = 0; i < Sequences.Count; i++ )
           {
               sequenceListView.UpdateLayout();
           }
           sequenceListView.ItemsSource = Sequences;
           

       }

       //Items were added or removed
       void Elements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
       {
           //Attach the observer for the properties
           if (e.NewItems != null)
               foreach (var vm in e.NewItems.OfType<FragmentViewModel>())
                   vm.PropertyChanged += Element_PropertyChanged;

           //Refresh when it is possible
           if (!this.Sequences.IsAddingNew && !this.Sequences.IsEditingItem)
               this.Sequences.Refresh();

           
       }

       private ObservableCollection<FragmentViewModel> Elements { get; set; }

       public ListCollectionView Sequences { get; set; }

       
       private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
       {
           this.InitializeSequenceListView();
       }

       private void sequenceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
       {
           
           if (sequenceListView.SelectedItem != null)
           {
               FragmentViewModel fvm = (FragmentViewModel)Sequences.GetItemAt(sequenceListView.SelectedIndex);
               String id = fvm.Name;
               sequenceName = id; 
               select = Design.Designer.FragmentDict[id];
               sequence = select.GetString();
               int max = (int)select.Length;

               sequenceTextBox.Text = createProDNA(sequence, aminoCode);
               Point eCenter = new Point(130, 130);
               Circle.drawSeqCircle(circleCanvas, eCenter, 150, 0, 0, max, arrowsData);

               infoTexBox.Text = "Name: " + (select.Name.ToString()) + Environment.NewLine + "Length: " + select.Length + Environment.NewLine;

               
           }
           else
           {
               sequenceTextBox.Text = "";
           }

       }




// zaznaczanie tekstu na kole
       private void sequencerSimple_PreviewMouseUp(object sender, MouseButtonEventArgs e)
       {
           caretStart = sequenceTextBox.CaretIndex;
           String selectedText = sequenceTextBox.SelectedText;

           int[] tuple = recalculatePos(caretStart, selectedText.Length);
           String selectedDNA = getSelected(tuple, sequence);
           selectedDNAs = selectedDNA;
           //MessageBox.Show(selectedDNA);
           //selectedDNAtextBox.Text = selectedDNA;
           int start = tuple[0];
           int end = tuple[1];
           int size = end - start + 1;
           int max = sequence.Length;

           Point eCenter = new Point(130, 130);
           Circle.drawSeqCircle(circleCanvas, eCenter, 150, start + 1, size, max, arrowsData);
       }
//blokowy textbox i wszystkie metody z nim związane----------------------------------------------------------------
       public String reverseDNA(String dna)
       {
           String result = "";
           for (int s = 0; s < dna.Length; s++)
           {
               String letter = dna.Substring(s, 1);
               if (letter.Equals("A") || letter.Equals("a"))
                   result += "T";
               else if (letter.Equals("C") || letter.Equals("c"))
                   result += "G";
               else if (letter.Equals("G") || letter.Equals("g"))
                   result += "C";
               else
                   result += "A";
           }

           return result;
       }
       /* Ciąg znaków txt wydłużany do rozmiaru size o znaki sign.
        */
       public String normalizeStr(String txt, int size, String sign)
       {
           if (txt.Length >= size)
               return txt;
           int orgSize = txt.Length;
           for (int i = 0; i < size - orgSize; i++)
           {
               txt += sign;
           }

           return txt;
       }

       /*
       Przetwarza sekwencję na linie wyświetlane przez TextBox. Operuje na zmiennych globalnych klasy związanych z roznymi
       blokami tekstu.
       */
       public String createProDNA(String dna, Boolean amino)
       {
           String result = "";
           String tmp = dna;
           String reverseDNA = this.reverseDNA(dna);
           String blockSizeCounter = "";
           String blockSeparators = "";
           String blockDNA = "";
           String blockReverseDNA = "";

           String aminoUp = "";
           String aminoDwn = "";

           if (amino)
           {
               seqMultiLine = 6;
               DNAlineInBlock = 2;
           }
           else
           {
               seqMultiLine = 4;
               DNAlineInBlock = 1;

           }

           int lineLettersCounter = 0;
           //int blockCounter = 0;
           // int lettersGlobalCounter = 0;
           while (tmp.Length >= blockSize)
           {
               blockSizeCounter += this.normalizeStr("" + lineLettersCounter, blockSize + 1, " ");
               blockSeparators += this.normalizeStr("-", blockSize + 1, "-");

               String dnaBlock = tmp.Substring(0, blockSize);
               blockDNA += this.normalizeStr(dnaBlock, blockSize + 1, " ");
               tmp = tmp.Substring(blockSize);

               String dnaRevBlock = reverseDNA.Substring(0, blockSize);
               blockReverseDNA += this.normalizeStr(dnaRevBlock, blockSize + 1, " ");
               reverseDNA = reverseDNA.Substring(blockSize);

               if (amino)
               {
                   aminoUp += this.normalizeStr(decodeAmino(dnaBlock, true), blockSize + 1, ">");
                   aminoDwn += this.normalizeStr(decodeAmino(dnaRevBlock, false), blockSize + 1, "<");
               }
               lineLettersCounter += blockSize;
           }
           if (tmp.Length > 0)
           {
               blockSizeCounter += this.normalizeStr("" + lineLettersCounter, blockSize + 1, " ");
               blockDNA += this.normalizeStr(tmp, blockSize + 1, "-");
               blockReverseDNA += this.normalizeStr(reverseDNA, blockSize + 1, "-");
               blockSeparators += this.normalizeStr("-", blockSize + 1, "-");

               if (amino)
               {
                   aminoUp += this.normalizeStr(decodeAmino(tmp, true), blockSize + 1, ">");
                   aminoDwn += this.normalizeStr(decodeAmino(reverseDNA, false), blockSize + 1, "<");
               }
           }

           //np. 5 * 10 w bloku + 4 (5-1) - ta czworka to spacje rozdzialaje 5 blokow

           while (blockSizeCounter.Length > blockLineTotalLength + 1)
           {
               result += blockSizeCounter.Substring(0, blockLineTotalLength);
               blockSizeCounter = blockSizeCounter.Substring(blockLineTotalLength + 1); //wytnij spacje
               result += Environment.NewLine;

               if (amino)
               {
                   result += aminoUp.Substring(0, blockLineTotalLength);
                   aminoUp = aminoUp.Substring(blockLineTotalLength + 1);
                   result += Environment.NewLine;
               }

               result += blockDNA.Substring(0, blockLineTotalLength);
               blockDNA = blockDNA.Substring(blockLineTotalLength + 1);
               result += Environment.NewLine;

               result += blockReverseDNA.Substring(0, blockLineTotalLength);
               blockReverseDNA = blockReverseDNA.Substring(blockLineTotalLength + 1);
               result += Environment.NewLine;

               if (amino)
               {
                   result += aminoDwn.Substring(0, blockLineTotalLength);
                   aminoDwn = aminoDwn.Substring(blockLineTotalLength + 1);
                   result += Environment.NewLine;
               }

               result += blockSeparators.Substring(0, blockLineTotalLength);
               blockSeparators = blockSeparators.Substring(blockLineTotalLength + 1);
               result += Environment.NewLine;
           }

           if (blockSizeCounter.Length > 0)
           {
               result += this.normalizeStr(blockSizeCounter, blockLineTotalLength, "-");
               result += Environment.NewLine;
               if (amino)
               {
                   result += this.normalizeStr(aminoUp, blockLineTotalLength, "-");
                   result += Environment.NewLine;
               }
               result += this.normalizeStr(blockDNA, blockLineTotalLength, "-");
               result += Environment.NewLine;
               result += this.normalizeStr(blockReverseDNA, blockLineTotalLength, "-");
               result += Environment.NewLine;
               if (amino)
               {
                   result += this.normalizeStr(aminoDwn, blockLineTotalLength, "-");
                   result += Environment.NewLine;
               }

               result += this.normalizeStr(blockSeparators, blockLineTotalLength, "-");
               result += Environment.NewLine;
           }

           return result;
       }

       /* Z dna -> aminokwas, po kodonach. Jeśli forward = false, każde XYZ przed dekodowaniem jest zmieniane na chwile w ZYX
       */
       private String decodeAmino(String dna, Boolean forward)
       {
           String code = "";
           String tmp = dna;
           if (forward)
           {
               while (tmp.Length > 2)
               {
                   code += codonToAminoAcid(tmp.Substring(0, 3));
                   tmp = tmp.Substring(3);
               }
           }
           else
           {
               while (tmp.Length > 2)
               {
                   code += codonToAminoAcid(reverseString(tmp.Substring(0, 3)));
                   tmp = tmp.Substring(3);
               }
           }
           return code;
       }

       /* Z łancucha znakow abcd robi dcba
       */
       public static string reverseString(string s)
       {
           char[] charArray = s.ToCharArray();
           Array.Reverse(charArray);
           return new string(charArray);
       }

       /* Dostaje kodon, zwraca aminokwas.
       */
       public string codonToAminoAcid(String codon)
       {
           for (int k = 0; k < CODONS.Length; k++)
           {
               if (CODONS[k].Equals(codon))
               {
                   if (!aminoFullNames)
                       return AMINOS_PER_CODON[k] + "  ";
                   //if  (!aminoYN)
                   //    return "  ";     
                   else
                       return AMINOS_PER_CODON_FULL[k];
               }
           }

           // never reach here with valid codon
           return "X";
       }
       /* Obsługa zaznaczenia tekstu po puszczeniu myszy
       */
       private void sequencerSimple_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
       {

           //zakładamy, że prawdziwe DNA ZAWSZE JEST W ZMIENNEJ sequence
           caretStart = sequenceTextBox.CaretIndex;
           String selectedText = sequenceTextBox.SelectedText;

           int[] tuple = recalculatePos(caretStart, selectedText.Length);
           String selectedDNA = getSelected(tuple, sequence); //jakby bylo potrzebne
           selectedDNAs = selectedDNA;
           //MessageBox.Show(selectedDNA);
           //selectedDNAtextBox.Text = selectedDNA;

           int start = tuple[0];
           int end = tuple[1];
           int size = end - start + 1;
           int max = sequence.Length;

           Point eCenter = new Point(130, 130);
           Circle.drawSeqCircle(circleCanvas, eCenter, 150, start + 1, size, max, arrowsData);

       }

       private string getSelected(int[] tuple, string sequence)
       {
           int start = tuple[0];
           int end = tuple[1];
           int size = end - start;
           if (size < 0)
               return "";
           try
           {
               return sequence.Substring(start, size + 1);
           }
           catch (Exception)
           {
               try
               {
                   return sequence.Substring(start); //lepsze to niż wywalenie się programu
               }
               catch (Exception) //FFS...
               {
                   return "";
               }
           }
       }

       /*Przelicz pozycję kursora i rozmiar zaznaczonego tekstu na punkt startu i konca zaznaczania prawdziwego DNA 
       */
       private int[] recalculatePos(int caretStart, int size)
       {
           int[] tuple = new int[2];

           int lineStartNumber = caretStart / (blockLineTotalLength + 2); //enter
           int startingMultiBlock = lineStartNumber / seqMultiLine; //linie liczone od zera
           int startingLetterFromLeft = caretStart % (blockLineTotalLength + 2); //w zasadzie to numer nastepnej litery w linii liczac od zera
           int startHowManySpaces = startingLetterFromLeft / (blockSize + 1);

           int endCaret = caretStart + size;

           int lineEndNumber = endCaret / (blockLineTotalLength + 2); //enter
           int endingMultiBlock = lineEndNumber / seqMultiLine; //linie liczone od zera
           int endingLetterFromLeft = endCaret % (blockLineTotalLength + 2); //patrz: startingLetterFromLeft
           int endHowManySpaces = endingLetterFromLeft / (blockSize + 1);

           int dnaRealStart = ((startingMultiBlock) * (blocksInLineCounter * blockSize)) + (startingLetterFromLeft - startHowManySpaces);
           int dnaRealEnd = ((endingMultiBlock) * (blocksInLineCounter * blockSize)) + (endingLetterFromLeft - endHowManySpaces) - 1;

           if (lineStartNumber != lineEndNumber && startingMultiBlock == endingMultiBlock)
           {
               tuple[0] = dnaRealStart;
               int howManyNotSelected = (blocksInLineCounter * blockSize) - dnaRealStart;
               tuple[1] = dnaRealStart + howManyNotSelected - 1;

           }
           else
           {
               tuple[0] = dnaRealStart;
               tuple[1] = dnaRealEnd;
           }

           if (size > 0)
           {
               int breakP = 1;
           }

           return tuple;
       }
 
//Obsługa dodawania i odejmowania literek z sequencerDNAtextBox
       private void sequencerSimple_TextChanged(object sender, TextChangedEventArgs e)
       {
           if (doNotUpdate)
               return;

           try
           {
               int caretPos = sequenceTextBox.CaretIndex;
               int lineStartNumber = caretPos / (blockLineTotalLength + 2); //enter
               int startingMultiBlock = lineStartNumber / seqMultiLine; //linie liczone od zera
               if (lineStartNumber % seqMultiLine == DNAlineInBlock)
               {
                   int startingLetterFromLeft = caretPos % (blockLineTotalLength + 2); //w zasadzie to numer nastepnej litery w linii liczac od zera
                   int startHowManySpaces = startingLetterFromLeft / (blockSize + 1);

                   int currentSize = sequenceTextBox.Text.Length;
                   doNotUpdate = true;
                   if (currentSize > textSize) //dodano coś
                   {
                       int index = startingLetterFromLeft - startHowManySpaces; //pozycja w linii
                       int preIndex = ((startingMultiBlock) * (blocksInLineCounter * blockSize));
                       String addedLetter = sequenceTextBox.Text.Substring(caretPos - 1, 1);
                       String left = sequence.Substring(0, preIndex + index - 1);
                       String right = sequence.Substring(preIndex + index - 1);
                      
                       sequence = left + addedLetter.ToUpper() + right;
                       sequenceTextBox.Text = createProDNA(sequence, aminoCode);
                       sequenceTextBox.CaretIndex = caretPos;
                   }
                   else if (currentSize < textSize) //usunięto coś
                   {
                       int index = startingLetterFromLeft - startHowManySpaces; //pozycja w linii
                       int preIndex = ((startingMultiBlock) * (blocksInLineCounter * blockSize));
                       String left = sequence.Substring(0, preIndex + index);
                       String right = sequence.Substring(preIndex + index + 1);

                       sequence = left + right;
                       sequenceTextBox.Text = createProDNA(sequence, aminoCode);
                       sequenceTextBox.CaretIndex = caretPos;

                   }
                   doNotUpdate = false;
                   textSize = sequenceTextBox.Text.Length;
                   //czyli jeśli wprowadzono cokolwiek w linii DNA
               }
               else
               {
                   doNotUpdate = true;
                   sequenceTextBox.Text = createProDNA(sequence, aminoCode);
                   sequenceTextBox.CaretIndex = caretPos;
                   doNotUpdate = false;
                   return;
               }
           }
           catch (Exception)
           {

           }
       }

// opcje menu ---------------------------------------------------------------------------------------

       private void alignButton_Click(object sender, RoutedEventArgs e)
       {
           if (sequenceListView.ItemsSource !=null)
           {
               AlignmentsWindow alignmentdWindow = new AlignmentsWindow();
               alignmentdWindow.Show();
             
           }
           else { };
       }


       private void check_trans_no(object sender, RoutedEventArgs e)
       {
           if (aminoCode == true)
           {
               aminoCode = false;

               if (aminoCode)
               {
                   seqMultiLine = 6;
                   DNAlineInBlock = 2;
               }
               else
               {
                   seqMultiLine = 4;
                   DNAlineInBlock = 1;

               }

               sequence = select.GetString();
               sequenceTextBox.Text = createProDNA(sequence, aminoCode);


               caretStart = sequenceTextBox.CaretIndex;
               String selectedText = sequenceTextBox.SelectedText;

               int[] tuple = recalculatePos(caretStart, selectedText.Length);
               String selectedDNA = getSelected(tuple, sequence); //jakby bylo potrzebne
               selectedDNAs = selectedDNA;
               int start = tuple[0];
               int end = tuple[1];
               int size = end - start + 1;
               int max = sequence.Length;

               Point eCenter = new Point(130, 130);
               Circle.drawSeqCircle(circleCanvas, eCenter, 150, start + 1, size, max, arrowsData);
           }
       }

       private void check_trans_long(object sender, RoutedEventArgs e)
       {

           aminoCode = true;
           aminoFullNames = true;
           if (aminoCode)
           {
               seqMultiLine = 6;
               DNAlineInBlock = 2;
           }
           else
           {
               seqMultiLine = 4;
               DNAlineInBlock = 1;

           }
           sequence = select.GetString();
           sequenceTextBox.Text = createProDNA(sequence, aminoCode);
 

       }

       private void check_trans_short(object sender, RoutedEventArgs e)
       {
           aminoCode = true;
           aminoFullNames = false;
           if (aminoCode)
           {
               seqMultiLine = 6;
               DNAlineInBlock = 2;
           }
           else
           {
               seqMultiLine = 4;
               DNAlineInBlock = 1;

           }
           sequence = select.GetString();
           sequenceTextBox.Text = createProDNA(sequence, aminoCode);

    
       }

       private void editButton_Click(object sender, RoutedEventArgs e)
       {
           if (selectedDNAs != null)
           {
               ModernWindow1 selectedWindow = new ModernWindow1(selectedDNAs);
               selectedWindow.Show();
               //selectedWindow.sequence = selectedDNAs;
           }
           else { };
       }




       private void check_circle(object sender, RoutedEventArgs e)
       {
           //circleCanvas.Children.Clear();
           circleCanvas.Visibility = Visibility.Visible;
       }

       private void unchecked_circle(object sender, RoutedEventArgs e)
       {
           circleCanvas.Visibility = Visibility.Hidden;
       }

//zapisywanie  ---------------------
       private void InitializeSaveSequenceFileDialog()
       {
           this.saveSequenceFileDialog = new Microsoft.Win32.SaveFileDialog();
           this.saveSequenceFileDialog.FileName = ""; // Default file name
           this.saveSequenceFileDialog.DefaultExt = ".gb"; // Default file extension
           this.saveSequenceFileDialog.Filter = "GenBank files| *.gb;*.gbk|Fasta files|*.fa;*.fas;*.fasta|All files|*.*"; // Filter files by extension

           this.saveSequenceFileDialog.Title = "Save sequence...";

       }
       private void saveButton_Click(object sender, RoutedEventArgs e)
       {
           if (sequenceTextBox.Text !=null)
           {
               InitializeSaveSequenceFileDialog();

               if (saveSequenceFileDialog.ShowDialog() == true)
                   try
                   {
                       Fragment toSave = new Fragment ();
                       toSave.Name = sequenceName;
                       ISequence seq= new Bio.Sequence( Alphabets.DNA, sequence);
                       toSave.Sequence = seq;
                       toSave.Length = sequence.Length;
                       ISequenceFormatter formatter = SequenceFormatters.FindFormatterByFileName(saveSequenceFileDialog.FileName);
                       formatter.Write(seq);
                       formatter.Close();
                       //toSave.Construct.SaveAsBio(saveSequenceFileDialog.FileName);
                   }
                   catch (Exception ex)
                   {
                       MessageBoxResult result = ModernDialog.ShowMessage(ex.Message, "Exception", MessageBoxButton.OK);
                   }
           }


       }


        // ku pamięci

        //private void sequenceTextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    sequenceTextBox.AppendText("" + Mouse.GetPosition(sequenceTextBox));
        //    Mouse.Capture(null);
        //}
    }
}
