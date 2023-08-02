using CoffeeShopMVC.DataAccess;
using CoffeeShopMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopMVC.Controllers
{
    public class ItemsController : Controller
    {
        private readonly CoffeeShopMVCContext _context;

        public ItemsController(CoffeeShopMVCContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var items = _context.Items.Include(i => i.Order).Where(i => i.Order == null).ToList();
            return View(items);
        }

        [Route("/items/details/{id:int}")]
        public IActionResult Show(int id)
        {
            var item = _context.Items.Find(id);

            return View(item);
        }

        [HttpPost]
        [Route("/items/details/{id:int}/delete")]
        public IActionResult Delete(int id)
        {
            var item = _context.Items.Find(id);
            _context.Items.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult New()
        {
            return View();
        }


        [HttpPost]
        [Route("/items/new")]
        public IActionResult Create(Item item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        
        [Route("/items/edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            var item = _context.Items.Find(id);
            return View(item);
        }

        [HttpPost]
        [Route("items/details/{id:int}")]
        public IActionResult Update(int id, Item item)
        {
            item.Id = id;
            _context.Items.Update(item);
            _context.SaveChanges();
            return Redirect($"/items/details/{id}");
        }
    }
}
