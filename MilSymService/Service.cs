// --------------------------------------------------------------------------------------------------------------------
// <copyright company="CF" file="Service.cs">
//   Copyright © 2012 CF
// </copyright>
// <summary>
//   Service definition for the MilSym PNG service.
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
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.ServiceProcess;
    using System.Threading;
    using System.Web;
    using System.Windows.Media;
    using MilSym.MilSymbol;
    using Properties;

    /// <summary>
    /// Service definition for the "MilSymbol" PNG service.
    /// To exercise, try a URL like: http://localhost:8765/milsym/SJAPMFRZ--MS***?scale=0.20
    /// </summary>
    public class Service : ServiceBase
    {
        /// <summary>
        /// The title for the MilSymbol service.
        /// </summary>
        public const string Name = "Military Symbol Service";

        /// <summary>
        /// The base url for the military symbol service.
        /// </summary>
        public const string BaseUrl = "/milsym/";

        /// <summary>
        /// The listener.
        /// </summary>
        private HttpListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        public Service()
        {
            this.ServiceName = Name;
            this.CanStop = true;
        }

        /// <summary>
        /// The service manager.
        /// </summary>
        public static void Main()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            // ReSharper disable once RedundantNameQualifier
            ServiceBase.Run(new Service());
        }

        /// <summary>
        /// Start up the listener when the service starts.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected override void OnStart(string[] args)
        {
            this.listener = new HttpListener();
            this.listener.Prefixes.Add(string.Format("http://*:{0}{1}", Settings.Default.Port, BaseUrl));
            this.listener.Start();

            var thread = new Thread(this.HandleRequests) { IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        /// <summary>
        /// Method for shutting down the service listener.
        /// </summary>
        protected override void OnStop()
        {
            this.listener.Stop();
            this.listener.Close();
        }

        /// <summary>
        /// The handler for service requests.
        /// </summary>
        private void HandleRequests()
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = this.listener.GetContext();

                    // This is typical of what Cesium wants to see.
                    //
                    //HTTP/1.1 200 OK
                    //Cache-Control: max-age=3600
                    //Content-Type: image/png
                    //Last-Modified: Thu, 16 May 2013 14:32:43 GMT
                    //Accept-Ranges: bytes
                    //ETag: "80f7b7394252ce1:0"
                    //Server: Microsoft-IIS/8.0
                    //X-UA-Compatible: IE=Edge,chrome=1
                    //Access-Control-Allow-Origin: *
                    //Date: Tue, 28 Jan 2014 14:37:23 GMT
                    //Content-Length: 1005
                    //
                    // So we'll  add some headers
                    context.Response.AddHeader("Cache-Control", "max-age=518400, public");      // 6D * 24H * 60M * 60S
                    context.Response.AddHeader("Content-Type", "image/png");
                    context.Response.AddHeader("Access-Control-Allow-Origin", "*");             // this is the important one
                    
                    if (context.Request.RawUrl.StartsWith(BaseUrl))
                    {
                        // Create a storage path, but skip the "/milsym/"
                        var path = @"images\" + HttpUtility.UrlEncode(context.Request.RawUrl.Substring(BaseUrl.Length)) + ".png";
                        path = path.Replace('*', '_');
                        bool haveBitmap;
                        if (File.Exists(path))
                        {
                            var fs = File.OpenRead(path);
                            fs.CopyTo(context.Response.OutputStream);
                            haveBitmap = true;
                        }
                        else
                        {
                            string symbolIdCode = context.Request.Url.AbsolutePath.Substring(BaseUrl.Length);
                            string queryString = context.Request.Url.Query;
                            double scale = 0.14;
                            string labelString = string.Empty;
                            double opacity = 1.0;
                            Brush fillBrush = null;
                            Brush lineBrush = null;

                            MilBrush.ColorScheme = ColorSchemeProperty.Light;

                            if (!string.IsNullOrEmpty(queryString))
                            {
                                string[] options = queryString.Substring(1).Split(new[] { '&' });
                                foreach (var option in options)
                                {
                                    var index = option.IndexOf("=", StringComparison.Ordinal);
                                    if (index <= 0)
                                    {
                                        continue;
                                    }

                                    var action = option.Substring(0, index).ToLower();
                                    switch (action)
                                    {
                                        case "fillcolor":
                                            try
                                            {
                                                var color =
                                                    System.Drawing.ColorTranslator.FromHtml(option.Substring(index + 1));
                                                fillBrush =
                                                    new SolidColorBrush(
                                                        Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                            // ReSharper disable once EmptyGeneralCatchClause
                                            catch (Exception)
                                            {
                                                 /* we don't care - we have a default null brush */
                                            }

                                            break;
                                        case "linecolor":
                                            try
                                            {
                                                var color =
                                                    System.Drawing.ColorTranslator.FromHtml(option.Substring(index + 1));
                                                lineBrush =
                                                    new SolidColorBrush(
                                                        Color.FromArgb(color.A, color.R, color.G, color.B));
                                            }
                                            // ReSharper disable once EmptyGeneralCatchClause
                                            catch (Exception)
                                            {
                                                 /* we don't care - we have a default null brush */
                                            }

                                            break;
                                        case "scale":
                                            if (
                                                !double.TryParse(
                                                    option.Substring(index + 1),
                                                    NumberStyles.AllowDecimalPoint,
                                                    CultureInfo.InvariantCulture,
                                                    out scale))
                                            {
                                                scale = 0.14;
                                            }

                                            break;
                                        case "labelstring":
                                            labelString = Uri.UnescapeDataString(option.Substring(index + 1));
                                            break;
                                        case "opacity":
                                            if (
                                                !double.TryParse(
                                                    option.Substring(index + 1),
                                                    NumberStyles.AllowDecimalPoint,
                                                    CultureInfo.InvariantCulture,
                                                    out opacity))
                                            {
                                                opacity = 1.0;
                                            }

                                            break;
                                        case "colorscheme":
                                            var scheme = option.Substring(index + 1).ToLower();
                                            switch (scheme)
                                            {
                                                case "dark":
                                                    MilBrush.ColorScheme = ColorSchemeProperty.Dark;
                                                    break;
                                                case "medium":
                                                    MilBrush.ColorScheme = ColorSchemeProperty.Medium;
                                                    break;
                                            }

                                            break;
                                    }
                                }
                            }

                            haveBitmap = MilSymBitmap.GetSymbolBuffer(
                                symbolIdCode, scale, labelString, opacity, lineBrush, fillBrush, path, context);
                        }

                        if (haveBitmap)
                        {
                            context.Response.ContentType = "image/png";
                            context.Response.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }

                    context.Response.OutputStream.Close();
                    context.Response.Close();
                }
                catch (HttpListenerException exc)
                {
                    Trace.TraceWarning(exc.ToString());
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(ex.ToString());
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
