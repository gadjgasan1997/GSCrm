namespace GSCrm
{
    public static class CommonConsts
    {
        // Пути
        public const string APP_INFO = "AppInfo";
        public const string ORG_VIEWS_REL_PATH = "~/Views/" + ORGANIZATION + "/";
        public const string AUTH_VIEWS_REL_PATH = "~/Views/" + AUTH + "/";
        public const string EMP_VIEWS_REL_PATH = "~/Views/" + EMPLOYEE + "/";
        public const string POS_VIEWS_REL_PATH = "~/Views/" + POSITION + "/";
        public const string ACC_VIEWS_REL_PATH = "~/Views/" + ACCOUNT + "/";
        public const string QT_VIEWS_REL_PATH = "~/Views/" + QUOTE + "/";
        public const string AUTH = "Auth";
        public const string USER = "User";

        // Организация
        public const string ORGANIZATION = "Organization";
        public const string ORGANIZATIONS = "Organizations";
        public const string RESPONSIBILITY = "Responsibility";
        public const string RESPONSIBILITIES = "Responsibilities";
        public const string DIVISION = "Division";
        public const string DIVISIONS = "Divisions";
        public const string POSITION = "Position";
        public const string POSITIONS = "Positions";

        // Сотрудник
        public const string EMPLOYEE = "Employee";
        public const string EMPLOYEES = "Employees";
        public const string EMP_POSITION = "EmployeePosition";
        public const string EMP_POSITIONS = "EmployeePositions";
        public const string CONTACT = "Contact";
        public const string CONTACTS = "Contacts";
        public const string EMP_CONTACT = "EmployeeContact";
        public const string EMP_CONTACTS = "EmployeeContacts";
        public const string ALL_EMP_POSS = "AllEmployeePositions";
        public const string SELECTED_EMP_POSS = "SelectedEmployeePositions";
        public const string SUBSORDINATE = "Subordinate";
        public const string SUBORDINATES = "Subordinates";
        public const string EMP_SUB = "EmployeeSubordinate";
        public const string EMP_SUBS = "EmployeeSubordinates";

        // Должность
        public const string POS_EMPLOYEE = "PositionEmployee";
        public const string POS_EMPLOYEES = "PositionEmployees";
        public const string POS_SUB_POS = "PositionSubPosition";
        public const string POS_SUB_POSS = "PositionSubPositions";

        // Клиент
        public const string ACCOUNT = "Account";
        public const string ACCOUNTS = "Accounts";
        public const string ALL_ACCS = "AllAccounts";
        public const string CURRENT_ACCS = "CurrentAccounts";
        public const string ACC_MANAGER = "AccountManager";
        public const string ACC_TEAM_ALL_EMPLOYEES = "AccTeamAllEmployees";
        public const string ACC_TEAM_SELECTED_EMPLOYEES = "AccTeamSelectedEmployees";
        public const string ACC_CONTACT = "AccountContact";
        public const string ACC_CONTACTS = "AccountContacts";
        public const string INVOICE = "Invoice";
        public const string INVOICES = "Invoices";
        public const string ACC_INVOICE = "AccountInvoice";
        public const string ACC_INVOICES = "AccountInvoices";
        public const string ADDRESS = "Address";
        public const string ADDRESSES = "Addresses";
        public const string ACC_ADDRESS = "AccountAddress";
        public const string ACC_ADDRESSES = "AccountAddresses";
        public const string QUOTE = "Quote";
        public const string QUOTES = "Quotes";
        public const string ACC_QUOTE = "AccountQuote";
        public const string ACC_QUOTES = "AccountQuotes";
        public const string DOCUMENT = "Document";
        public const string DOCUMENTS = "Documents";
        public const string ACC_DOC = "AccountDocument";
        public const string ACC_DOCS = "AccountDocuments";

        // Сделка
        public const string ALL_QUOTES = "AllQuotes";
        public const string CURRENT_QUOTES = "CurrentQuotes";

        // Прочие
        public const string REGION_KEY = "Region";
        public const string CITY_KEY = "City";
        public const string STREET_KEY = "Street";
        public const string HOUSE_KEY = "House";

        // Числовые
        public const int DEFAULT_ITEMS_COUNT = 10;
        public const int DEFAULT_MIN_PAGE_NUMBER = 1;
        public const int DEFAULT_PAGE_STEP = 1;
        public const int DEFAULT_BREAK_COUNTER = 5;
    }
}
