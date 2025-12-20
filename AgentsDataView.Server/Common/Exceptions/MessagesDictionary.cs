namespace AgentsDataView.Common.Exceptions
{
    public static class MessagesDictionary
    {
        public static readonly Dictionary<string, string> Messages = new()
        {
        {
                "access denied",
                "دسترسی مجاز نیست."
        },
        {
                "the requesting user is invalid.",
                "کاربر درخواست کننده معتبر نیست."
        },
        {
                "the requesting company's authorization has expired.",
                "اعتبار شرکت درخواست کننده به پایان رسیده است."
        },
        {
                "the requesting company is invalid.",
                "شرکت درخواست کننده معتبر نیستو"
        },
        {
                "this token has no claims.",
                "توکن ارسالی معتبر نیست."
        },
        {
                "authentication failed.",
                "اشکال در احراز هویت"
        },
        {
                "you are unauthorized to access this resource.",
                "دسترسی مجاز نیست."
        },
        {
                "invalid refresh token",
                "توکن بازیابی معتبر نیست"
        }
        };
    }
}
