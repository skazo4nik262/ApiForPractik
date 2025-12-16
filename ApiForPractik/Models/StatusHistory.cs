namespace ApiForPractik.Models
{
    // Пример модели истории статуса
    public class StatusHistory
    {
        public int Id { get; set; }
        public string PreviousStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
        public int ChangedByUserId { get; set; } // ID пользователя, изменившего статус
        public int ApplicationRequestId { get; set; }
        public ApplicationRequest ApplicationRequest { get; set; } = null!;
    }

}
