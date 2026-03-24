namespace SuryaPolyFlex.Application.Common;

public static class Permissions
{
    public static class Departments
    {
        public const string View   = "DEPARTMENTS_VIEW";
        public const string Create = "DEPARTMENTS_CREATE";
        public const string Edit   = "DEPARTMENTS_EDIT";
        public const string Delete = "DEPARTMENTS_DELETE";
    }

    public static class Employees
    {
        public const string View   = "EMPLOYEES_VIEW";
        public const string Create = "EMPLOYEES_CREATE";
        public const string Edit   = "EMPLOYEES_EDIT";
        public const string Delete = "EMPLOYEES_DELETE";
    }

    public static class Users
    {
        public const string View   = "USERS_VIEW";
        public const string Create = "USERS_CREATE";
        public const string Edit   = "USERS_EDIT";
        public const string Delete = "USERS_DELETE";
    }

    public static class Vendors
    {
        public const string View   = "VENDORS_VIEW";
        public const string Create = "VENDORS_CREATE";
        public const string Edit   = "VENDORS_EDIT";
        public const string Delete = "VENDORS_DELETE";
    }

    public static class Items
    {
        public const string View   = "ITEMS_VIEW";
        public const string Create = "ITEMS_CREATE";
        public const string Edit   = "ITEMS_EDIT";
        public const string Delete = "ITEMS_DELETE";
    }

    public static class Indents
    {
        public const string View    = "INDENTS_VIEW";
        public const string Create  = "INDENTS_CREATE";
        public const string Edit    = "INDENTS_EDIT";
        public const string Delete  = "INDENTS_DELETE";
        public const string Approve = "INDENTS_APPROVE";
    }

    public static class PurchaseOrders
    {
        public const string View    = "PO_VIEW";
        public const string Create  = "PO_CREATE";
        public const string Edit    = "PO_EDIT";
        public const string Delete  = "PO_DELETE";
        public const string Approve = "PO_APPROVE";
    }

    public static class GRN
    {
        public const string View   = "GRN_VIEW";
        public const string Create = "GRN_CREATE";
        public const string Edit   = "GRN_EDIT";
    }

    public static class Stock
    {
        public const string View   = "STOCK_VIEW";
        public const string Adjust = "STOCK_ADJUST";
        public const string Issue  = "STOCK_ISSUE";
        public const string Export = "STOCK_EXPORT";
    }

    public static class Customers
    {
        public const string View   = "CUSTOMERS_VIEW";
        public const string Create = "CUSTOMERS_CREATE";
        public const string Edit   = "CUSTOMERS_EDIT";
        public const string Delete = "CUSTOMERS_DELETE";
    }

    public static class SalesOrders
    {
        public const string View    = "SO_VIEW";
        public const string Create  = "SO_CREATE";
        public const string Edit    = "SO_EDIT";
        public const string Approve = "SO_APPROVE";
    }

    public static class Quotations
    {
        public const string View   = "QTN_VIEW";
        public const string Create = "QTN_CREATE";
        public const string Edit   = "QTN_EDIT";
    }

    public static class JobCards
    {
        public const string View   = "JC_VIEW";
        public const string Create = "JC_CREATE";
        public const string Edit   = "JC_EDIT";
    }

    public static class WorkOrders
    {
        public const string View   = "WO_VIEW";
        public const string Create = "WO_CREATE";
        public const string Edit   = "WO_EDIT";
    }

    public static class Production
    {
        public const string View   = "PROD_VIEW";
        public const string Create = "PROD_CREATE";
        public const string Edit   = "PROD_EDIT";
    }

    public static class Dispatch
    {
        public const string View   = "DISPATCH_VIEW";
        public const string Create = "DISPATCH_CREATE";
        public const string Edit   = "DISPATCH_EDIT";
        public const string Export = "DISPATCH_EXPORT";
    }

    public static class Reports
    {
        public const string View   = "REPORTS_VIEW";
        public const string Export = "REPORTS_EXPORT";
    }

    public static class AuditLogs
    {
        public const string View = "AUDIT_VIEW";
    }
}