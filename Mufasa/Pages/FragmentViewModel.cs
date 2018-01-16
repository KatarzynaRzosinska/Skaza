using Bio;
using Mufasa.BackEnd.Designer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mufasa.Pages
{
    /// <summary>
    /// Wraps Fragment class and provide notification of changes
    /// </summary>
    public class FragmentViewModel
    {
        /// <summary>
        /// FragmentViewModel constructor.
        /// </summary>
        public FragmentViewModel()
        {
            this.Model = new Fragment();
        }

        /// <summary>
        /// FragmentViewModel constructor.
        /// </summary>
        /// <param name="m">Fragment model.</param>
        public FragmentViewModel(Fragment m)
        {
            this.Model = m;
        }

        /// <value>
        /// Fragment model.
        /// </value>
        public Fragment Model { get; private set; }

        /// <summary>
        /// Concentration.
        /// </summary>
        public double Concentration
        {
            get { return this.Model.Concentration; }
            set
            {
                this.Model.Concentration = value;
                OnPropertyChanged("Concentration");
            }
        }

        /// <summary>
        /// Volume.
        /// </summary>
        public double Volume
        {
            get { return this.Model.Volume; }
            set
            {
                this.Model.Volume = value;
                OnPropertyChanged("Volume");
            }
        }

        /// <summary>
        /// True if a vector fragment.
        /// </summary>
        public bool IsVector
        {
            get { return this.Model.IsVector; }
            set
            {
                this.Model.IsVector = value;
                OnPropertyChanged("IsVector");
            }
        }

        /// <summary>
        /// Fragment length.
        /// </summary>
        public long Length
        {
            get { return this.Model.Length; }
            set
            {
                this.Model.Length = value;
                OnPropertyChanged("Length");
            }
        }

        /// <value>
        /// Path to the file or url containing the fragment.
        /// </value>
        public String Source
        {
            get { return this.Model.Source; }
            set
            {
                this.Model.Source = value;
                OnPropertyChanged("Source");
            }
        }
        /// <summary>
        /// Name of the fragment.
        /// </summary>
        public String Name
        {
            get { return this.Model.Name; }
            set
            {
                this.Model.Name = value;
                OnPropertyChanged("Name");
            }
        }
        /// <summary>
        /// Fragment sequence.
        /// </summary>
        public ISequence Sequence
        {
            get { return this.Model.Sequence; }
            set
            {
                this.Model.Sequence = value;
                OnPropertyChanged("Sequence");
            }
        }

        /// <summary>
        /// PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// PropertyChanged trigger.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
