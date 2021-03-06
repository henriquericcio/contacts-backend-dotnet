using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsBackendDotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContactsBackendDotnet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly ContactContext _context;

        public ContactsController(ILogger<ContactsController> logger, ContactContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> Get()
        {
            return await _context.Contacts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> Get(Guid id)
        {
            var item = await _context.Contacts.FindAsync(id);

            if (item == null)
                return NotFound();

            return item;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Contact value)
        {
            _context.Contacts.Add(value);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = value.Id }, value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Contact value, Guid id)
        {
            if (!id.Equals(value.Id))
                return BadRequest();

            var item = await _context.Contacts.FindAsync(id);
            if (item == null)
                return NotFound();

            item.FirstName = value.FirstName;
            item.LastName = value.LastName;
            item.PhoneNumber = value.PhoneNumber;

            _context.Contacts.Update(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _context.Contacts.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.Contacts.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}