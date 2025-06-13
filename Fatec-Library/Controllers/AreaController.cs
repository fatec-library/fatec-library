using Fatec_Library.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Fatec_Library.Controllers
{

    public class AreaController : Controller
    {
        private readonly ContextMongodb _context;

        public AreaController()
        {
            _context = new ContextMongodb();
        }

        public async Task<IActionResult> Listar()
        {
            var area = await _context.Areas.Find(p => true).ToListAsync();
            return View(area);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Area area)
        {
            if (ModelState.IsValid)
            {
                await _context.Areas.InsertOneAsync(area);
                return RedirectToAction(nameof(Listar));
            }

            return View(area);
        }

        public IActionResult Delete(string id)
        {
            var area = _context.Areas.Find(a => a.Id == id).FirstOrDefault();
            return View(area);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Area area)
        {
            if (ModelState.IsValid)
            {
                await _context.Areas.DeleteOneAsync(a => a.Id == area.Id);
                return RedirectToAction(nameof(Listar));
            }

            return View(area);
        }

    }

}
