using Excel.IService;
using Excel.VM;

namespace Excel.AppService
{
    public class NotificationService : INotificationService
    {

        public Task NotifyUserAsync(ExportNotification notification)
        {
            // 简单演示：控制台输出
            Console.WriteLine($"通知用户 {notification.UserId}：文件 “{notification.FileName}” 导出完成，时间 {notification.Timestamp}");
            return Task.CompletedTask;
        }
    }

}
