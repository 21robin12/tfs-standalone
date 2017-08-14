using System;
using System.Windows.Forms;

namespace TfsStandalone.UI
{
    using System.Drawing;
    using System.IO;
    using CefSharp;
    using CefSharp.WinForms;
    using Controllers;
    using RazorEngine;
    using RazorEngine.Templating;

    public partial class BrowserForm : Form
    {
        public static BrowserForm Instance;

        private readonly ChromiumWebBrowser _browser;

        public BrowserForm()
        {
            InitializeComponent();
            Text = "TFS Standalone";

            var settings = new CefSettings();
            Cef.Initialize(settings);

            _browser = new ChromiumWebBrowser(string.Empty) {Dock = DockStyle.Fill};
            browserPanel.Controls.Add(_browser);

            Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon-small.ico"));
            Width = 800;
            Height = 600;

            _browser.IsBrowserInitializedChanged += BrowserOnIsBrowserInitializedChanged;
            _browser.RegisterAsyncJsObject("csharpAjax", new AjaxObject());

            Instance = this;
        }

        public void Render<T>(T model, string view)
        {
            var service = Engine.Razor;
            
            // TODO add a nice way to read all all the files in the views folder and include them automatically
            var templates = new[]
            {
                "Views/Shared/_Layout.cshtml",
                "Views/Partials/_ShelvesetDiff.cshtml",
                "Views/Partials/_UnmergedChangesets.cshtml",
                "Views/Partials/_Settings.cshtml"
            };

            foreach (var t in templates)
            {
                var layout = File.ReadAllText(t);
                service.AddTemplate(t, layout);
            }

            // Note that "key" is the template key, which Razor uses to cache the compiled result.
            // We can re-run the cached template using this same key: var html = Engine.Razor.Run("key", typeof(MyViewModel), vm); 
            var key = view;
            var template = File.ReadAllText(view);
            var html = Engine.Razor.RunCompile(template, key, typeof(T), model);

            // "\\placeholder" tricks CefSharp into using the entire directory path; otherwise it strips off the last component
            // TODO fix this - maybe I'm just linking incorrectly
            var directory = Directory.GetCurrentDirectory() + "\\placeholder";
            _browser.LoadHtml(html, directory);
        }

        public void ExecuteJsFunction(string function, params object[] args)
        {
            _browser.ExecuteScriptAsync(function, args);

            //_browser.FrameLoadEnd += (sender2, args) =>
            //{
            //    if (args.Frame.IsMain)
            //    {
            //        args.Frame.ExecuteJavaScriptAsync($"init({data})");
            //    }
            //};
        }

        private void BrowserOnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs isBrowserInitializedChangedEventArgs)
        {
            if (_browser.IsBrowserInitialized)
            {
                if (CustomCefSettings.ShowDevTools)
                {
                    _browser.ShowDevTools();
                }
                
                new MainController(new Services.AuthenticationService()).Main();
            }
        }
    }
}
