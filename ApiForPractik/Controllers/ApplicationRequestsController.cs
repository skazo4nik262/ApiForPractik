using ApiForPractik.Data;
using ApiForPractik.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiForPractik.Data;
using ApiForPractik.Models;

namespace ApiForPractik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationRequestsController : ControllerBase
    {
        private readonly RequestContext _context;
 
        public ApplicationRequestsController(RequestContext context)
        {
            _context = context;
        }

        // GET: api/ApplicationRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationRequest>>> GetApplicationRequests(
            [FromQuery] string? status,
            [FromQuery] int? assigneeId,
            [FromQuery] int? requesterId,
            [FromQuery] string sortBy = "dateCreated", // По умолчанию сортировка по дате создания
            [FromQuery] string sortOrder = "desc",     // По умолчанию по убыванию
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var query = _context.ApplicationRequests.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }
            if (assigneeId.HasValue)
            {
                query = query.Where(r => r.AssigneeId == assigneeId.Value);
            }
            if (requesterId.HasValue)
            {
                query = query.Where(r => r.RequesterId == requesterId.Value);
            }

            // Сортировка
            switch (sortBy.ToLower())
            {
                case "status":
                    query = sortOrder.ToLower() == "asc" ? query.OrderBy(r => r.Status) : query.OrderByDescending(r => r.Status);
                    break;
                case "datecreated":
                default:
                    query = sortOrder.ToLower() == "asc" ? query.OrderBy(r => r.DateCreated) : query.OrderByDescending(r => r.DateCreated);
                    break;
            }

            // Пагинация
            var requests = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(requests);
        }

        // GET: api/ApplicationRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationRequest>> GetApplicationRequest(int id)
        {
            var applicationRequest = await _context.ApplicationRequests.FindAsync(id);

            if (applicationRequest == null)
            {
                return NotFound();
            }

            return Ok(applicationRequest);
        }

        // POST: api/ApplicationRequests
        [HttpPost]
        public async Task<ActionResult<ApplicationRequest>> PostApplicationRequest(ApplicationRequest applicationRequest)
        {
            // Валидация минимальная на стороне сервера
            if (string.IsNullOrWhiteSpace(applicationRequest.Title) || string.IsNullOrWhiteSpace(applicationRequest.Description))
            {
                return BadRequest("Title and Description are required.");
            }

            applicationRequest.DateCreated = DateTime.UtcNow;
            applicationRequest.DateUpdated = DateTime.UtcNow;

            _context.ApplicationRequests.Add(applicationRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApplicationRequest), new { id = applicationRequest.Id }, applicationRequest);
        }

        // PUT: api/ApplicationRequests/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> PutApplicationRequestStatus(int id, [FromBody] string newStatus)
        {
            if (string.IsNullOrEmpty(newStatus))
            {
                return BadRequest("New status cannot be empty.");
            }

            var applicationRequest = await _context.ApplicationRequests.FindAsync(id);
            if (applicationRequest == null)
            {
                return NotFound();
            }

            // Логика обновления статуса и даты
            var oldStatus = applicationRequest.Status;
            applicationRequest.Status = newStatus;
            applicationRequest.DateUpdated = DateTime.UtcNow;

            // Создание записи в истории статусов (пример)
            var statusHistoryEntry = new StatusHistory
            {
                PreviousStatus = oldStatus,
                NewStatus = newStatus,
                ChangeDate = DateTime.UtcNow,
                ChangedByUserId = 1, // Заменить на реальный ID пользователя из JWT или контекста
                ApplicationRequestId = applicationRequest.Id
            };
            _context.StatusHistories.Add(statusHistoryEntry);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/ApplicationRequests/5
        // Для частичного обновления можно использовать PATCH, но для простоты пусть будет PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplicationRequest(int id, ApplicationRequest applicationRequest)
        {
            if (id != applicationRequest.Id)
            {
                return BadRequest();
            }

            // Валидация минимальная на стороне сервера
            if (string.IsNullOrWhiteSpace(applicationRequest.Title) || string.IsNullOrWhiteSpace(applicationRequest.Description))
            {
                return BadRequest("Title and Description are required.");
            }

            // Обновляем дату изменения
            applicationRequest.DateUpdated = DateTime.UtcNow;

            _context.Entry(applicationRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        private bool ApplicationRequestExists(int id)
        {
            return _context.ApplicationRequests.Any(e => e.Id == id);
        }
    }
}