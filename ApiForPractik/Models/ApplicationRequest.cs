using System.ComponentModel.DataAnnotations;

namespace ApiForPractik.Models
{
    public class ApplicationRequest
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)] // Для enum или строкового значения статуса
        public string Status { get; set; } = "NEW"; // Значение по умолчанию

        [MaxLength(20)] // Для enum или строкового значения
        public string Priority { get; set; } = "MEDIUM";

        public int? RequesterId { get; set; } // Может быть null, если автор неизвестен или не требуется
        public int? AssigneeId { get; set; }  // Может быть null, если не назначен

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; } = DateTime.UtcNow;

        public int? CategoryId { get; set; } // Ссылка на категорию (опционально)

        // Свойства навигации (не мапятся напрямую в колонки)
        // public User? Requester { get; set; }
        // public User? Assignee { get; set; }
        // public Category? Category { get; set; }
        public List<Attachment>? Attachments { get; set; } // Список вложений
        public List<StatusHistory>? StatusHistory { get; set; } // История изменений статуса
    }

}
