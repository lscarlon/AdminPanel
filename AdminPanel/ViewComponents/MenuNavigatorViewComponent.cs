using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminPanel.Models;


namespace AdminPanel.ViewComponents
{
    public class MenuNavigatorViewComponent : ViewComponent
    {
        private readonly AppDbContext db;

        public MenuNavigatorViewComponent(AppDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await GetItemsAsync();
            return View(items);
        }

        private Task<List<Menu>> GetItemsAsync()
        {
            return db.Menus.Where(m => m.DisplayOrder>=0).ToListAsync();
        }
    }
}
