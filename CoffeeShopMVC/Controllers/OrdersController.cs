using CoffeeShopMVC.DataAccess;
using CoffeeShopMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopMVC.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CoffeeShopMVCContext _context;

        public OrdersController(CoffeeShopMVCContext context)
        {
            _context = context;
        }

        [Route("/customers/{customerId:int}/orders")]
        public IActionResult Index(int customerId, bool? toggledelete)
        {
            if(toggledelete != null)
            {
                ViewData["ToggleDelete"] = true;
            }
            var customer = _context.Customers.Where(c => c.Id == customerId).Include(c => c.Orders).ThenInclude(o => o.Items).First();
            return View(customer);
        }

        [Route("/customers/{customerId:int}/orders/details/{orderId:int}")]
        public IActionResult Show(int customerId, int orderId)
        {
            var customer = _context.Customers.Where(c => c.Id == customerId).Include(c => c.Orders).ThenInclude(o => o.Items).First();
            var order = customer.Orders.Where(o => o.Id == orderId).First();
            order.Customer = customer;
            return View(order);
        }


        [Route("/customers/{customerId:int}/orders/new")]
        public IActionResult New(int customerId)
        {
            var customer = _context.Customers.Where(c => c.Id == customerId).Include(c => c.Orders).First();
            return View(customer);
        }

        [HttpPost]
        [Route("/customers/{customerId:int}/orders")]
        public IActionResult Create(Order order, int customerId)
        {
            var customer = _context.Customers.Where(c => c.Id == customerId).Include(c => c.Orders).ThenInclude(o => o.Items).First();
            order.DateCreated = order.DateCreated.ToUniversalTime();
            customer.Orders.Add(order);
            _context.SaveChanges();

            return Redirect($"/customers/{customer.Id}/orders");
        }

        [Route("/customers/{customerId:int}/orders/edit/{orderId:int}")]
        public IActionResult Edit(int customerId, int orderId)
        {
            var customer = _context.Customers.Where(c => c.Id == customerId).Include(c => c.Orders).ThenInclude(o => o.Items).First();
            var order = customer.Orders.Where(o => o.Id == orderId).First();
            return View(order);
        }

        [HttpPost]
        [Route("/customers/{customerId:int}/orders/details/{orderId:int}")]
        public IActionResult Update(int customerId, int orderId, Order order)
        {
            order.Id = orderId;
            order.DateCreated = order.DateCreated.ToUniversalTime();
            _context.Orders.Update(order);
            _context.SaveChanges();
            return Redirect($"/customers/{customerId}/orders/details/{orderId}");
        }

        [HttpPost]
        [Route("/customers/{customerId:int}/orders/delete/{orderId:int}")]
        public IActionResult Delete(int customerId, int orderId)
        {
            var order = _context.Orders.Find(orderId);
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return Redirect($"/customers/{customerId}/orders");
        }

        [Route("/customers/{customerId:int}/orders/additem/{orderId:int}")]
        public IActionResult AddItem(int customerId, int orderId)
        {
            var order = _context.Orders.Include(o => o.Items).Include(o => o.Customer).Where(o => o.Id == orderId).First();
            
            var items = _context.Items.Include(i => i.Order).ToList();

            ViewData["allitems"] = items;
            return View(order);
        }

        [HttpPost]
        [Route("/customers/{customerId:int}/orders/additem/{orderId:int}")]
        public IActionResult AddItem(int customerId, int orderId, int itemId)
        {
            var order = _context.Orders.Include(o => o.Items).Where(o => o.Id == orderId).First();
            var item = _context.Items.Find(itemId);
            order.Items.Add(item);
            _context.Orders.Update(order);
            _context.SaveChanges();

            return Redirect($"/customers/{customerId}/orders/additem/{orderId}");
        }

    }
}
