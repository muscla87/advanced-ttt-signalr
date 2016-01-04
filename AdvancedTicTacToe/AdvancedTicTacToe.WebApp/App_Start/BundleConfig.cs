using System.Web.Optimization;

namespace AdvancedTicTacToe.WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                      "~/Scripts/angular.js",
                      "~/Scripts/angular-animate.js",
                      "~/Scripts/angular-route.js",
                      "~/Scripts/angular-signalr-hub.js"));

            bundles.Add(new ScriptBundle("~/signalr/library").Include(
                      "~/Scripts/jquery.signalR-2.2.0.js"));

            bundles.Add(new ScriptBundle("~/bundles/thegame")
                            .IncludeDirectory("~/ClientApp", "*.js", true));

            bundles.Add(new StyleBundle("~/Content/css___").Include(
                //"~/Content/bootstrap.css",
                //"~/Content/hover.css",
                      "~/Content/site.css",
                      "~/Content/game.css",
                      "~/Content/login.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/main.css"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //         "~/Content/main_test.css"));
        }
    }
}
