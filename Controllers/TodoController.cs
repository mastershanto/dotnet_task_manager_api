using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoController(TodoContext context)
    {
        _context = context;
    }

    // GET: api/todo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTodoItems()
    {
        return await _context.Tasks.ToListAsync();
    }

    // GET: api/todo/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItem>> GetTodoItem(int id)
    {
        var todoItem = await _context.Tasks.FindAsync(id);

        if (todoItem == null)
        {
            return NotFound();
        }

        return todoItem;
    }

    // POST: api/todo
    [HttpPost]
    public async Task<ActionResult<TaskItem>> PostTodoItem(TaskItem todoItem)
    {
        todoItem.CreatedAt = DateTime.UtcNow;
        _context.Tasks.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
    }

    // PUT: api/todo/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(int id, TaskItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return BadRequest();
        }

        var existingItem = await _context.Tasks.FindAsync(id);
        if (existingItem == null)
        {
            return NotFound();
        }

        existingItem.Title = todoItem.Title;
        existingItem.Description = todoItem.Description;
        existingItem.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(existingItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/todo/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {
        var todoItem = await _context.Tasks.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
