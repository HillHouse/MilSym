// --------------------------------------------------------------------------------------------------------------------
// <copyright company="HillHouse" file="DescData.cs">
//   Copyright © 2009-2012 HillHouse
// </copyright>
// <summary>
//   Core data structure for user friendly descriptions of symbol codes, used to define a tree hierarchy for those codes.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSym.MilSymbol
{
    /// <summary>
    /// A helper structure that holds the description and the parent template name for a given template. 
    /// </summary>
    internal struct DescData
    {
        /// <summary>
        /// The description of a given template, derived from the 2525C documentation.
        /// </summary>
        public string Description;

        /// <summary>
        /// The symbol code for the parent template.
        /// </summary>
        public string ParentCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="DescData"/> struct.
        /// </summary>
        /// <param name="d">
        /// The description for a template.
        /// </param>
        /// <param name="p">
        /// The parent code for a template.
        /// </param>
        public DescData(string d, string p)
        {
            this.Description = d;
            this.ParentCode = p;
        }
    }
}