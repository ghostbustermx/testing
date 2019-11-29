using System.Web;
using System.Web.Optimization;

namespace Locust
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            #region Styles
            bundles.Add(new StyleBundle("~/Content/template").Include(
                "~/Scripts/vendors/bootstrap/dist/css/bootstrap.min.css",
                "~/Scripts/vendors/font-awesome/css/font-awesome.min.css",
                "~/Scripts/vendors/css/custom.min.css",
                "~/Scripts/vendors/css/toggleButton.css",
                "~/Scripts/vendors/css/ngToast.min.css",
                "~/Scripts/vendors/css/loading-bar.min.css",
                "~/Scripts/vendors/ngToast/stacktable.css",
                "~/Scripts/vendors/css/angular-datepicker.css",
                "~/Scripts/vendors/css/angular-tooltips.css",
                "~/Scripts/vendors/ng-table.min.css",
                "~/Scripts/vendors/bootstrap-toggle-master/css/bootstrap-toggle.min.css",
                "~/Scripts/vendors/ngTags/ng-tags-input.min.css",
                "~/Scripts/vendors/css/table-fixed.css",
                "~/Scripts/vendors/css/style.css",
                "~/Scripts/vendors/bootstrap-select/select.css",
                "~/Scripts/vendors/bootstrap-select/selectize.default.css",
                "~/Scripts/vendors/bootstrap-select/select2.css"
                ));
            #endregion Styles


            #region Scripts
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/vendors/angularjs/angular.js",
                "~/Scripts/vendors/angularjs/angular-ui-router.js",
                "~/Scripts/vendors/angularjs/angular-sanitize.js",
                 "~/Scripts/vendors/angularjs/angular-animate.js",
                "~/Scripts/vendors/angularjs/angular-messages.js",
                "~/Scripts/vendors/angular-ui/ui-bootstrap-tpls.js",
                "~/Scripts/vendors/angularjs/angular-resource.js",
                "~/Scripts/vendors/angularjs/angular-datepicker.js",
                "~/Scripts/vendors/ngToast/ngToast.min.js",
                "~/Scripts/vendors/angularjs/loading-bar.min.js",
                "~/Scripts/vendors/ngToast/stacktable.js",
                "~/Scripts/vendors/angular-tooltips/angular-tooltips.js",
                "~/Scripts/vendors/dirPagination/dirPagination.js",
                "~/Scripts/vendors/bootstrap-toggle-master/js/bootstrap-toggle.min.js",
                 "~/Scripts/vendors/bootstrap-select/select.min.js",
                 "~/Scripts/vendors/jquery-ui/jquery-ui.js",
                "~/Scripts/vendors/sortable/sortable.js",
                "~/Scripts/vendors/ng-table.min.js",
                "~/Scripts/vendors/TableDnD-masyer/dist/jquery.tablednd.js",
                "~/Scripts/vendors/jquery.tagsinput/src/jquery.tagsinput.js",
                "~/Scripts/vendors/ngTags/ng-tags-input.min.js",
                "~/Scripts/vendors/angularjs-dropdown-multiselect/dist/src/angularjs-dropdown-multiselect.js",
                                "~/Scripts/vendors/wizard/wizard.js",
                                 "~/Scripts/vendors/ngAnimate/angularAnimate.js"
               ));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/app/app.js",
                "~/Scripts/app/uiStates.js",
                "~/Scripts/app/services/*.js",
                "~/Scripts/app/directives/*.js",
                "~/Scripts/app/controllers/*.js"));


            bundles.Add(new ScriptBundle("~/bundles/template").Include(
                "~/Scripts/vendors/jquery/dist/jquery.min.js",
                "~/Scripts/vendors/bootstrap/dist/js/bootstrap.min.js",
                "~/Scripts/vendors/js/custom.js",
                "~/Scripts/vendors/js/html2canvas.js",
                "~/Scripts/vendors/ng-table/ng-table.js",
                "~/Scripts/vendors/ng-table/ng-table.src.js",
                "~/Scripts/ng-table.min.js",
                "~/Scripts/ng-table.js",
                "~/Scripts/vendors/TableDnD-masyer/dist/jquery.tablednd.js"));

            bundles.Add(new ScriptBundle("~/bundles/AdjustUI").Include(
              "~/Scripts/vendors/js/adjustUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/AdjustUIWizard").Include(
            "~/Scripts/vendors/js/adjustUIWizard.js"));


            #endregion Scripts
        }
    }
}
