// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="Symbols.cs">
//   Copyright © 2009-2011 HillHouse
// </copyright>
// <summary>
//   The core code for the symbol bindings.
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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// The class that contains the symbols to be displayed.
    /// </summary>
    public class Symbols : INotifyPropertyChanged
    {
        /// <summary>
        /// Vertical distance between symbols.
        /// </summary>
        private const int Wide = 100;

        /// <summary>
        /// Horizontal distance between symbols.
        /// </summary>
        private const int Loose = 75;

        /// <summary>
        /// The four most common affiliations.
        /// </summary>
        private static readonly string[] CommonAffiliations = new[] { "U", "H", "N", "F" };

        /// <summary>
        /// Random number generator to assist in initializing the symbols.
        /// </summary>
        private static readonly Random Rand = new Random(42);

        /// <summary>
        /// A timer for periodic updates.
        /// </summary>
        private static DispatcherTimer dt;

        /// <summary>
        /// Create the property that will be the source of the binding.
        /// </summary>
        private ObservableCollection<ISymbol> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="Symbols"/> class.
        /// </summary>
        public Symbols()
        {
            // Kick off a timer to do some stuff
            if (dt == null)
            {
                dt = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 100) };
                dt.Tick += this.DtTick;
                dt.Start();
            }
        }

        /// <summary>
        /// The standard PropertyChangedEventHandler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets Items.
        /// </summary>
        public ObservableCollection<ISymbol> Items
        {
            get
            {
                return this.items;
            }

            set
            {
                if (this.items == value)
                {
                    return;
                }

                this.items = value;
                this.NotifyPropertyChanged("Items");
            }
        }

        /// <summary>
        /// Initialize the items collection with some random Symbol data
        /// </summary>
        public void LoadData()
        {
            var temp = new ObservableCollection<ISymbol>();

            double x = Loose;
            double y = Loose;

            var affiliations = new[] { "F", "H" };
            var categoryBattleDimension = new[] { "A", "G", "U" };
            var mob = new[] { "MO", "MQ", "MS", "H-", "HB", "FK" };

            foreach (string ee in affiliations)
            {
                foreach (string bd in categoryBattleDimension)
                {
                    foreach (string m in mob)
                    {
                        string sc = "I" + ee + bd + "ASRU---" + m + "AF*";
                        temp.Add(
                            new Symbol(
                                sc + "\nat (" + x + "," + y + ")",
                                sc,
                                new Point(x, y),
                                360 * Rand.NextDouble()));
                        x += Wide;
                        if (x > 6 * Wide)
                        {
                            y += Wide;
                            x = Loose;
                        }
                    }
                }
            }

            this.Items = temp;
        }

        /// <summary>
        /// The standard NotifyPropertyChanged.
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

        /// <summary>
        /// The timer event to show how items can bind to this collection
        /// </summary>
        /// <param name="sender">The DispatcherTimer.</param>
        /// <param name="e">This parameter is not used.</param>
        private void DtTick(object sender, EventArgs e)
        {
            var dtick = sender as DispatcherTimer;
            if (dtick == null)
            {
                return;
            }

            dtick.Stop();
            if (this.Items == null || this.Items.Count == 0)
            {
                this.LoadData();
                dtick.Start();
            }
            else
            {
                this.AnimateData();
                dtick.Start();
            }
        }

        /// <summary>
        /// Modify the symbol code and angle for a random subset of the items
        /// </summary>
        private void AnimateData()
        {
            foreach (ISymbol item in this.Items)
            {
                double d = Rand.NextDouble();
                if (d > 0.5)
                {
                    var k = (int)((d - 0.5) * 8);
                    if (k > 3)
                    {
                        k = 3;
                    }

                    item.SymbolCode = item.SymbolCode.Substring(0, 1) +
                        CommonAffiliations[k] +
                        item.SymbolCode.Substring(2);
                    item.AngleDegrees += 5;
                    item.Name =
                        item.SymbolCode + "\nat (" +
                        item.Position.X + "," + item.Position.Y + ")";
                }
            }
        }
    }
}