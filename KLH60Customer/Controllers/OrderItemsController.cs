using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KLH60Customer.Models;
using Microsoft.AspNetCore.Authorization;

namespace KLH60Customer.Controllers
{
    [Authorize(Roles = "Customer")]
    public class OrderItemsController : Controller
    {
        //    // GET: OrderItems
        //    public async Task<IActionResult> Index()
        //    {
        //        var storeCustContext = _context.OrderItems.Include(o => o.Orders);
        //        return View(await storeCustContext.ToListAsync());
        //    }

        //    // GET: OrderItems/Details/5
        //    public async Task<IActionResult> Details(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var orderItem = await _context.OrderItems
        //            .Include(o => o.Orders)
        //            .FirstOrDefaultAsync(m => m.OrderItemId == id);
        //        if (orderItem == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(orderItem);
        //    }

        //    // GET: OrderItems/Create
        //    public IActionResult Create()
        //    {
        //        ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
        //        return View();
        //    }

        //    // POST: OrderItems/Create
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("OrderItemId,OrderId,ProductId,Quantity,Price")] OrderItem orderItem)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(orderItem);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(orderItem);
        //    }

        //    // GET: OrderItems/Edit/5
        //    public async Task<IActionResult> Edit(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var orderItem = await _context.OrderItems.FindAsync(id);
        //        if (orderItem == null)
        //        {
        //            return NotFound();
        //        }
        //        return View(orderItem);
        //    }

        //    // POST: OrderItems/Edit/5
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(int id, [Bind("OrderItemId,OrderId,ProductId,Quantity,Price")] OrderItem orderItem)
        //    {
        //        if (id != orderItem.OrderItemId)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(orderItem);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (Exception e)
        //            {
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }

        //        return View(orderItem);
        //    }

        //    // GET: OrderItems/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var orderItem = await _context.OrderItems
        //            .Include(o => o.Orders)
        //            .FirstOrDefaultAsync(m => m.OrderItemId == id);
        //        if (orderItem == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(orderItem);
        //    }

        //    // POST: OrderItems/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var orderItem = await _context.OrderItems.FindAsync(id);
        //        _context.OrderItems.Remove(orderItem);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private IActionResult GoToGenericError(Exception e)
        //    {
        //        ViewData["err"] = e.Message;
        //        return View("GenericError");
        //    }
    }
}