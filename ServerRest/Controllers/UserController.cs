using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerRest.DataBase;

namespace ServerRest.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserTable>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var users = await _context.UserTable.ToListAsync();
        return Ok(users);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserTable), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _context.UserTable.FindAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserTable), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] UserTable user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        await _context.UserTable.AddAsync(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new {id = user.Id}, user);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserTable), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(int id, [FromBody] UserTable user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != user.Id)
        {
            return BadRequest();
        }
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(UserTable), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.UserTable.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        _context.UserTable.Remove(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }
}