namespace AgentsDataView.Common.Utilities
{
    public static class DateExtensions
    {
        public static string ToPersian(this DateTime date)
        {
            var g = new System.Globalization.PersianCalendar();
            if (date > DateTime.MinValue)
            {

                return string.Format("{0}/{1}/{2}", g.GetYear(date), g.GetMonth(date).ToString("00"), g.GetDayOfMonth(date).ToString("00"));
            }
            return "";
        }
        public static string ToPersian(this DateTime? date)
        {
            var g = new System.Globalization.PersianCalendar();
            if (date > DateTime.MinValue)
            {

                return string.Format("{0}/{1}/{2}", g.GetYear(date.Value), g.GetMonth(date.Value).ToString("00"), g.GetDayOfMonth(date.Value).ToString("00"));
            }
            return "";
        }

        public static bool HasValue(this DateTime? date)
        {
            return date != null && date > DateTime.MinValue;
        }
        public static bool HasValue(this DateTime date)
        {
            return date > DateTime.MinValue;
        }
    }
}
