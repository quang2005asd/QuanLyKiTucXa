using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

public class InvoicesController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public InvoicesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<InvoiceDto>>> GetInvoices(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<InvoiceDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Invoices.CountAsync();
        var invoices = await _context.Invoices
            .Include(i => i.Contract)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoices);
        return OkPaginatedResponse(invoiceDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<InvoiceDto>>> GetInvoice(int id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Contract)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (invoice == null)
            return NotFoundResponse<InvoiceDto>("Invoice not found");

        var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
        return OkResponse(invoiceDto);
    }

    [HttpGet("contract/{contractId}")]
    public async Task<ActionResult<PaginatedResponseDto<InvoiceDto>>> GetInvoicesByContract(int contractId, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<InvoiceDto>("Page number and page size must be greater than 0");

        // Check if contract exists
        var contractExists = await _context.Contracts.AnyAsync(c => c.Id == contractId);
        if (!contractExists)
            return NotFoundPaginatedResponse<InvoiceDto>("Contract not found");

        var totalCount = await _context.Invoices
            .Where(i => i.ContractId == contractId)
            .CountAsync();

        var invoices = await _context.Invoices
            .Where(i => i.ContractId == contractId)
            .Include(i => i.Contract)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoices);
        return OkPaginatedResponse(invoiceDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("unpaid")]
    public async Task<ActionResult<PaginatedResponseDto<InvoiceDto>>> GetUnpaidInvoices(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<InvoiceDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Invoices
            .Where(i => i.Status == "Unpaid" || i.Status == "Overdue")
            .CountAsync();

        var invoices = await _context.Invoices
            .Where(i => i.Status == "Unpaid" || i.Status == "Overdue")
            .Include(i => i.Contract)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var invoiceDtos = _mapper.Map<List<InvoiceDto>>(invoices);
        return OkPaginatedResponse(invoiceDtos, totalCount, pageNumber, pageSize);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<InvoiceDto>>> PostInvoice(CreateInvoiceDto createInvoiceDto)
    {
        var validationError = ValidateModelState<InvoiceDto>();
        if (validationError != null)
            return validationError;

        // Check if contract exists
        var contract = await _context.Contracts.FindAsync(createInvoiceDto.ContractId);
        if (contract == null)
            return BadRequestResponse<InvoiceDto>("Contract not found");

        // Check if invoice already exists for this month/year
        var existingInvoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.ContractId == createInvoiceDto.ContractId
                && i.Month == createInvoiceDto.Month
                && i.Year == createInvoiceDto.Year && !i.IsDeleted);

        if (existingInvoice != null)
            return BadRequestResponse<InvoiceDto>("Invoice already exists for this month/year");

        // Check if invoice number already exists
        if (await _context.Invoices.AnyAsync(i => i.InvoiceNumber == createInvoiceDto.InvoiceNumber))
            return BadRequestResponse<InvoiceDto>("Invoice number already exists");

        var invoice = _mapper.Map<Invoice>(createInvoiceDto);
        invoice.TotalAmount = invoice.RentAmount + invoice.ServiceAmount;
        invoice.CreatedAt = DateTime.UtcNow;

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
        return CreatedResponse("GetInvoice", new { id = invoice.Id }, invoiceDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<InvoiceDto>>> PutInvoice(int id, UpdateInvoiceDto updateInvoiceDto)
    {
        var validationError = ValidateModelState<InvoiceDto>();
        if (validationError != null)
            return validationError;

        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
            return NotFoundResponse<InvoiceDto>("Invoice not found");

        _mapper.Map(updateInvoiceDto, invoice);
        invoice.UpdatedAt = DateTime.UtcNow;

        try
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InvoiceExists(id))
                return NotFoundResponse<InvoiceDto>("Invoice not found");
            throw;
        }

        var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
        return OkResponse(invoiceDto, "Invoice updated successfully");
    }

    [HttpPut("{id}/mark-paid")]
    public async Task<ActionResult<ApiResponseDto<InvoiceDto>>> MarkAsPaid(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
            return NotFoundResponse<InvoiceDto>("Invoice not found");

        invoice.Status = "Paid";
        invoice.PaymentDate = DateTime.UtcNow;
        invoice.UpdatedAt = DateTime.UtcNow;

        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();

        var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
        return OkResponse(invoiceDto, "Invoice marked as paid successfully");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteInvoice(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null)
            return NotFoundResponse<object>("Invoice not found");

        // Soft delete
        invoice.IsDeleted = true;
        invoice.DeletedAt = DateTime.UtcNow;
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();

        return OkResponse<object>(new { }, "Invoice deleted successfully");
    }

    private bool InvoiceExists(int id)
    {
        return _context.Invoices.Any(e => e.Id == id);
    }
}
