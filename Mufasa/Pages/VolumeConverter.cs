using Mufasa.BackEnd.Designer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Mufasa.Pages
{
    /// <remarks>
    /// Volume converter class. Computes Reaction volumes from fragment lengths and concentrations.
    /// </remarks>
    class VolumeConverter : IMultiValueConverter
    {
        /// <summary>
        /// Valume convertion.
        /// </summary>
        /// <param name="values">Values produced by the FragmentViewModel.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter.</param>
        /// <param name="culture">Culture info.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType,
                    object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return null;

            var item = values[0] as FragmentViewModel;
            var view = values[1] as ICollectionView;
            if (item == null || view == null)
                return null;

            var volume = item.Length * 0.1 / item.Concentration;
            if (item.IsVector)
            {
                volume /= 2;
            }
            return Math.Round(volume,1).ToString();
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">Not implemented.</param>
        /// <param name="targetTypes">Not implemented.</param>
        /// <param name="parameter">Not implemented.</param>
        /// <param name="culture">Not implemented.</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
