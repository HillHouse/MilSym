// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Symbol.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   A sample business object.
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
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Pretend this is some sort of business object class
    /// </summary>
    public class Symbol : ISymbol, INotifyPropertyChanged
    {
        /// <summary>
        /// The angle at which to display the symbol.
        /// </summary>
        private double angleDegrees;

        /// <summary>
        /// The name to associate with the symbol.
        /// </summary>
        private string name;

        /// <summary>
        /// The location of the symbol.
        /// </summary>
        private Point position;

        /// <summary>
        /// The symbol code for the symbol.
        /// </summary>
        private string symbolCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Symbol"/> class.
        /// </summary>
        /// <param name="name">
        /// The name to associate with the symbol.
        /// </param>
        /// <param name="symbolCode">
        /// The symbol code.
        /// </param>
        /// <param name="position">
        /// The position of the symbol.
        /// </param>
        /// <param name="angleDegrees">
        /// The rotation angle in degrees.
        /// </param>
        public Symbol(string name, string symbolCode, Point position, double angleDegrees)
        {
            this.Name = name;
            this.SymbolCode = symbolCode;
            this.Position = position;
            this.AngleDegrees = angleDegrees;
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Standard PropertyChangedEventHandler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ISymbol Members

        /// <summary>
        /// Gets or sets the name associated with a symbol.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name == value)
                {
                    return;
                }

                this.name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets the symbol code.
        /// </summary>
        public string SymbolCode
        {
            get
            {
                return this.symbolCode;
            }

            set
            {
                if (this.symbolCode == value)
                {
                    return;
                }

                this.symbolCode = value;
                this.NotifyPropertyChanged("SymbolCode");
            }
        }

        /// <summary>
        /// Gets or sets the angle in degrees.
        /// </summary>
        public double AngleDegrees
        {
            get
            {
                return this.angleDegrees;
            }

            set
            {
                if (Math.Abs(this.angleDegrees - value) < double.Epsilon)
                {
                    return;
                }

                this.angleDegrees = value;
                this.NotifyPropertyChanged("AngleDegrees");
            }
        }

        /// <summary>
        /// Gets or sets the position, in pixel space.
        /// </summary>
        public Point Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (this.position == value)
                {
                    return;
                }

                this.position = value;
                this.NotifyPropertyChanged("Position");
            }
        }

        #endregion

        /// <summary>
        /// Standard NotifyPropertyChanged.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}