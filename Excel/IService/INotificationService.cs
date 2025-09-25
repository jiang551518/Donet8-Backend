using Excel.VM;

namespace Excel.IService
{
    public interface INotificationService
    {
        /// <summary>
        /// 根据通知内容异步通知用户
        /// </summary>
        Task NotifyUserAsync(ExportNotification notification);
    }

}
