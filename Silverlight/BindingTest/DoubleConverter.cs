// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="DoubleConverter.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   A sample converter.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace BindingTest
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Value converter to convert a double into a string - as an example only.
    /// </summary>
    public class DoubleConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Value converter to convert a double into a string - as an example only.
        /// </summary>
        /// <param name="ob">
        /// The object that is actually a double.
        /// </param>
        /// <param name="type">
        /// The type is not used.
        /// </param>
        /// <param name="parameter">
        /// The parameter is not used.
        /// </param>
        /// <param name="culture">
        /// The culture is not used.
        /// </param>
        /// <returns>
        /// A string that is the double value.
        /// </returns>
        public object Convert(object ob, Type type, object parameter, CultureInfo culture)
        {
            var item = (double)ob;
            return item.ToString("###°");
        }

        /// <summary>
        /// The reverse converter is not implemented.
        /// </summary>
        /// <param name="ob">
        /// The object is not used.
        /// </param>
        /// <param name="type">
        /// The type is not used.
        /// </param>
        /// <param name="parameter">
        /// The parameter is not used.
        /// </param>
        /// <param name="culture">
        /// The culture is not used.
        /// </param>
        /// <returns>
        /// Nothing since it throws an exception.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Only forward conversion is required.
        /// </exception>
        public object ConvertBack(object ob, Type type, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
