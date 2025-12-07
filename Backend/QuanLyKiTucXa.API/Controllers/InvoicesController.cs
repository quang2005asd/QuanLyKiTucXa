using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InvoicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
    {
        return await _context.Invoices
            .Include(i => i.Contract)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Invoice>> GetInvoice(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Contract)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (invoice == null)
        {
            return NotFound();
        }
        return invoice;
    }

    [HttpGet("contract/{contractId}")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByContract(int contractId)
    {
        return await _context.Invoices
            .Where(i => i.ContractId == contractId)
            .Include(i => i.Contract)
            .ToListAsync();
    }

    [HttpGet("unpaid")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetUnpaidInvoices()
    {
        return await _context.Invoices
            .Where(i => i.Status == "Unpaid" || i.Status == "Overdue")
            .Include(i => i.Contract)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
    {
        // Check if invoice already exists for this month/year
        var existingInvoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.ContractId == invoice.ContractId 
                && i.Month == invoice.Month 
                && i.Year == invoice.Year);
        
        if (existingInvoice != null)
        {
            return BadRequest("Invoice already exists for this month/year");
        }

        // Generate InvoiceNumber
        invoice.InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        invoice.TotalAmount = invoice.RentAmount + invoice.ServiceAmount;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
    {
        if (id != invoice.Id)
        {
            return BadRequest();
        }

        _context.Entry(invoice).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InvoiceExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpPut("{id}/mark-paid")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        invoice.Status = "Paid";
        invoice.PaymentDate = DateTime.UtcNow;

        _context.Entry(invoice).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
        {
            return NotFound();
        }

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool InvoiceExists(int id)
    {
        return _context.Invoices.Any(e => e.Id == id);
    }
}
