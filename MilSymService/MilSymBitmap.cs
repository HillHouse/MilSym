// --------------------------------------------------------------------------------------------------------------------
// <copyright company="CF" file="MilSymBitmap.cs">
//   Copyright © 2012 CF
// </copyright>
// <summary>
//   Returns a PNG-based byte array representing a MilSymbol code.
// </summary>
// <license>
//   Licensed under the Ms-PL license.
// </license>
// <homepage>
//   http://milsym.codeplex.com
// </homepage>
// --------------------------------------------------------------------------------------------------------------------

namespace MilSymService
{
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using MilSym.MilSymbol;

    /// <summary>
    /// Generates a PNG-based byte array representing a MilSymbol code.
    /// </summary>
    public static class MilSymBitmap
    {
        /// <summary>
        /// The padding to add if a symbol doesn't match the required 15 character size.
        /// </summary>
        private const string Padding = "----------*****";

        /// <summary>
        /// Generates a PNG-based byte array representing a MilSymbol code.
        /// </summary>
        /// <param name="symbolIdCode">
        /// The MilSymbol code.
        /// </param>
        /// <param name="scale">
        /// The scale, typically a number between 0 and 1.
        /// </param>
        /// <param name="labelString">
        /// The labels for the symbol.
        /// </param>
        /// <param name="opacity">
        /// The opacity for the symbol.
        /// </param>
        /// <param name="lineBrush">
        /// The line brush for coloring the symbol's outline.
        /// </param>
        /// <param name="fillBrush">
        /// The fill brush for coloring the symbol's background.
        /// </param>
        /// <param name="storagePath">
        /// The storage path for caching the PNG.
        /// </param>
        /// <param name="context">
        /// The request/response context.
        /// </param>
        /// <returns>
        /// A boolean indicating whether we have a bitmap.
        /// </returns>
        public static bool GetSymbolBuffer(
            string symbolIdCode, 
            double scale, 
            string labelString, 
            double opacity,
            Brush lineBrush,
            Brush fillBrush,
            string storagePath,
            HttpListenerContext context)
        {
            if (symbolIdCode.Length < 4 || symbolIdCode.Length > 15)
            {
                return false;
            }

            if (symbolIdCode.Length < 15)
            {
                symbolIdCode += Padding.Substring(symbolIdCode.Length);
            }

            var symbol = new MilSymbol(symbolIdCode, scale, labelString, lineBrush, fillBrush);

            if (symbol.Bounds == Rect.Empty)
            {
                symbolIdCode = symbolIdCode.Substring(0, 4) + Padding.Substring(4);
                symbol = new MilSymbol(symbolIdCode, scale, labelString, lineBrush, fillBrush);

                if (symbol.Bounds == Rect.Empty)
                {
                    return false;
                }
            }

            // Create a bigger pixel map if we're doing labels
            var size = (labelString.Length > 0) ? (int)(scale * 2000d) : (int)(scale * 600d);

            symbol.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var halfSize = size / 2;
            symbol.Arrange(new Rect(halfSize, halfSize, size, size));
            symbol.UpdateLayout();

            symbol.Opacity = opacity;

            var bitmap = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Default);
            bitmap.Render(symbol);
            
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // Cannot save the encoder output directly to the OutputStream - bummer
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                using (var file = new FileStream(storagePath, FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(file);
                }

                stream.WriteTo(context.Response.OutputStream);
                return true;
            }
        }
    }
}
