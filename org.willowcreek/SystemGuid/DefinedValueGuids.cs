namespace org.willowcreek.SystemGuid
{
    /// <summary>
    /// Static Guids used by the Willow Creek Rock application
    /// </summary>
    public static class DefinedValueGuids
    {
        #region Check-in Search Type

        /// <summary>
        /// Barcode search type
        /// </summary>
        public const string CHECKIN_SEARCH_TYPE_BARCODE = "739B0B4F-83E8-479E-A875-20357A1C9A55";

        #endregion

        #region Events
        /// <summary>
        /// Home address type
        /// </summary>
        public const string HOME_ADDRESS = "8C52E53C-2A66-435A-AE6E-5EE307D9A0DC";
        #endregion

        #region PersonAttributes
        /// <summary>
        /// Mobile phone type
        /// </summary>
        public const string MOBILE_PHONE = "407E7E45-7B2E-4FCD-9605-ECB1339F2453";
        /// <summary>
        /// Home phone type
        /// </summary>
        public const string HOME_PHONE = "AA8732FB-2CEA-4C76-8D6D-6AAA2C6A4303";
        /// <summary>
        /// Work phone type
        /// </summary>
        public const string WORK_PHONE = "2CC66D5A-F61C-4B74-9AF9-590A9847C13C";
        #endregion

        #region ConnectionStatus
        public const string MEMBER = "41540783-D9EF-4C70-8F1D-C9E83D91ED5F";
        public const string VISITOR = "B91BA046-BC1E-400C-B85D-638C1F4E0CE2";
        public const string WEB_PROSPECT = "368DD475-242C-49C4-A42C-7278BE690CC2";
        public const string ATTENDEE = "39F491C5-D6AC-4A9B-8AC0-C431CB17D588";
        public const string PARTICIPANT = "8EBC0CEB-474D-4C1B-A6BA-734C3A9AB061";
        public const string NEW = "7E52B310-ADF7-4EC0-8623-A98BD4CDC9DD";
        public const string REFERENCE = "0FA8D32F-492A-478F-8692-012BBC0B004C";
        public const string CARE_CENTER_GUEST = "AF7F9659-D047-4EB6-97D4-31034A5801BC";
        public const string PROMISELAND_GUEST = "953A224F-DCEE-40AB-989B-B376F91B6D1A";
        public const string PROMISELAND_FAMILY = "C074C222-3760-4E6D-AA24-64A0A72766BB";
        #endregion

        public static class YouthCovenantStatus
        {
            public const string PROCESS_INITIATED = "3B2FB5F8-CE29-4005-A89A-B3DBAF726E1F";
            public const string IN_PROGRESS = "9FC53424-7719-4EA3-BBE8-4393CEE91C96";
            public const string NEEDS_REVIEW = "523D007B-7591-4852-828C-9A6F4470E4E8";
            public const string APPROVED = "46146064-0B95-4D60-98BA-2E8844A59EBB";
            public const string INELIGIBLE = "87FF9E28-A819-42CE-B5EC-F0A462BAE18D";
            public const string EXPIRED = "B486ACB2-ABA2-4821-B287-4BE1003B0F3A";
        }

    }
}