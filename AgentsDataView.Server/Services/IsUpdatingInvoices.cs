namespace AgentsDataView.Server.Services
{
   
    public interface IIsUpdatingInvoices
    {
        public void SetFalse();
        public bool TrySetTrue();
        DateTime? LastUpdateTime { get; }

    }
    public class IsUpdatingInvoices: IIsUpdatingInvoices
    {
        private int _flag = 0; // 0 = false, 1 = true
        private readonly object _lock = new();
        public DateTime? LastUpdateTime { get; private set; }
        public bool TrySetTrue()
        {
            // اگر _flag برابر 0 باشد، مقدارش را 1 کن و true برگردان
            if (Interlocked.CompareExchange(ref _flag, 1, 0) == 0)
            {
                lock (_lock)
                {
                    LastUpdateTime = DateTime.UtcNow;
                }
                return true;
            }
            return false;   
        }

        public void SetFalse()
        {
            Interlocked.Exchange(ref _flag, 0);
        }

        public bool Flag => _flag == 1;
    }
}
