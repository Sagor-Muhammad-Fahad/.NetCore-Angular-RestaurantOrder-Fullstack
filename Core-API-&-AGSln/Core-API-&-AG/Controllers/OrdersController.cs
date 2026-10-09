using Core_API___AG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core_API___AG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly RestaurantDbContext _db;
        private readonly IWebHostEnvironment _env;

        public OrdersController(RestaurantDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }


        [HttpGet("Item/Include")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersWithDetails()
        {
            var orders = await _db.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}/Include")]
        public async Task<ActionResult<Order>> GetOrderByIdWithDetails(int id)
        {
            var order = await _db.Orders
                .Include(o => o.RestaurantTable)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _db.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            string? fullPath = null;
            if (!string.IsNullOrWhiteSpace(order.ReceiptImageUrl))
            {
                var fileName = Path.GetFileName(order.ReceiptImageUrl);
                fullPath = Path.Combine(_env.WebRootPath, "images", fileName);
            }

            _db.OrderItems.RemoveRange(order.OrderItems);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            if (fullPath != null && System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch (IOException ex)
                {

                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Order>> SaveOrder(Order order)
        {
            if (string.IsNullOrEmpty(order.ReceiptImageUrl))
            {
                order.ReceiptImageUrl = "noimage.png";
            }

            order.OrderItems = order.OrderItems ?? new List<OrderItem>();

            order.RestaurantTable = null!;

            foreach (var item in order.OrderItems)
            {
                item.Order = null;
                item.MenuItem = null;
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrdersWithDetails), new { id = order.OrderId }, order);
        }

        [HttpPost("Upload/{id}")]
        public async Task<ActionResult<UploadResponse>> Upload(int id, IFormFile file)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                return BadRequest("Only JPEG and PNG files are allowed");

            if (file.Length > 2 * 1024 * 1024)
                return BadRequest("File size exceed Limit");

            string ext = Path.GetExtension(file.FileName);
            string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
            string savePath = Path.Combine(_env.WebRootPath, "images", fileName);

            if (!Directory.Exists(Path.Combine(_env.WebRootPath, "images")))
            {
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images"));
            }

            try
            {
                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                order.ReceiptImageUrl = fileName;
                await _db.SaveChangesAsync();

                return Ok(new UploadResponse { FileName = order.ReceiptImageUrl });
            }
            catch (IOException ex)
            {
                Console.WriteLine($"File Upload Exception: {ex.Message}");
                return StatusCode(500, "Error occured to upload file");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.OrderId) return BadRequest();

            try
            {
                var existingOrder = await _db.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (existingOrder == null) return NotFound();

               
                _db.Entry(existingOrder).CurrentValues.SetValues(order);

                existingOrder.RestaurantTable = null!;

                if (order.OrderItems != null)
                {

                    var incomingIds = order.OrderItems
                    .Where(u => u.OrderItemId > 0)
                    .Select(u => u.OrderItemId)
                    .ToList();

                    var itemsToRemove = existingOrder.OrderItems
                        .Where(existing => !incomingIds.Contains(existing.OrderItemId))
                        .ToList();

                    foreach (var item in itemsToRemove)
                    {
                        _db.OrderItems.Remove(item);
                    }

                    foreach (var updateItem in order.OrderItems)
                    {

                        updateItem.Order = null;
                        updateItem.MenuItem = null;

                        var existingItem = updateItem.OrderItemId > 0
                            ? existingOrder.OrderItems.FirstOrDefault(i => i.OrderItemId == updateItem.OrderItemId)
                            : null;

                        if (existingItem != null)
                        {
                            _db.Entry(existingItem).CurrentValues.SetValues(updateItem);
                            
                        }
                        else
                        {
                            updateItem.OrderItemId = 0;
                            updateItem.OrderId = existingOrder.OrderId;
                            existingOrder.OrderItems.Add(updateItem);
                        }
                    }
                }

                await _db.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _db.Orders.Any(e => e.OrderId == id);
        }


        [HttpGet("restaurantTables")]
        public async Task<IActionResult> GetTablesForDropdown()
        {
            var tables = await _db.RestaurantTables
                //.AsNoTracking()
                //.Select(t => new
                //{
                //    t.RestaurantTableId,
                //    t.TableNumber,
                //    t.Capacity
                //})
                .ToListAsync();

            return Ok(tables);
        }

        [HttpGet("menuItems")]
        public async Task<IActionResult> GetMenuItemsForDropdown()
        {
            var items = await _db.MenuItems
                .Select(m => new
                {
                    m.MenuItemId,
                    m.ItemName
                })
                .ToListAsync();

            return Ok(items);
        }

    }
}
