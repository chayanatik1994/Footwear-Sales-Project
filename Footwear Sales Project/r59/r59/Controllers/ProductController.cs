using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using r59.Models;
using r59.ViewModels;
using System.CodeDom;
using System.Security.Claims;

namespace r59.Controllers
{
    public class ProductController(ProductDbContext db, UserManager<IdentityUser> user, IWebHostEnvironment env) : Controller
    {



        [Authorize(Roles= "Admin")]
        public IActionResult Index1()
        { 
        var data = db.Products.Include(x=> x.Sales).ToList();
        return View(data);
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = user.GetUserId(User);

            
            var data = db.Products.Where(x=> x.CreatedBy == userId).Include(x => x.Sales).ToList();
            return View(data);
            
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProductInputModel();
            model.Sales.Add(new Sale { });
            return View(model);
        
        }
        [HttpPost]
        public IActionResult Create(ProductInputModel model, string operation = "")
        {
            if (operation == "add")
            {
                model.Sales.Add(new Sale { });
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.RawValue = null;
                }
            }
            if (operation.StartsWith("del"))
            {
                int index = int.Parse(operation.Substring(operation.IndexOf("_") + 1));
                model.Sales.RemoveAt (index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.RawValue = null;
                }
            }
            if(operation== "insert")
            {
                if(ModelState.IsValid)
                {
                    
                    var p = new Product
                    { 
                    CreatedBy = user.GetUserId(User),
                    ProductName = model.ProductName,
                    ProductType = model.ProductType,
                    Price = model.Price,
                    MfgDate = model.MfgDate,
                    InStock = model.InStock,
                    
                    };
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string f = Path.GetFileNameWithoutExtension (Path.GetRandomFileName()) + ext;
                    string filePath = Path.Combine(env.WebRootPath, "Pictures", f);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    model.Picture.CopyTo(fs);
                   
                    fs.Close();
                    p.Picture = f;
                    foreach (var s in model.Sales)
                    { 
                    p.Sales.Add (s);
                    }
                    db.Products.Add (p);

                    db.SaveChangesAsync();
                    return RedirectToAction (nameof(Index));
                }
            }
            return View(model);

        }
        public IActionResult Edit(int id)
        {
            var Product = db.Products.Include(p => p.Sales).FirstOrDefault(x => x.ProductId == id);
            if (Product == null) return NotFound();
            var model = new ProductEditModel
            {
                ProductId = id,
                ProductName = Product.ProductName,
                ProductType = Product.ProductType,
                Price = Product.Price,
                MfgDate = Product.MfgDate,
                InStock = Product.InStock,
            };
            model.Sales = Product.Sales.ToList();
            ViewBag.CurrentPicture = Product.Picture;
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(ProductEditModel model, string operation = "")
        {
            var p = db.Products.FirstOrDefault(x => x.ProductId == model.ProductId);
            if (p == null) return NotFound();
            if (operation == "add")
            {
                model.Sales.Add(new Sale { });
                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                    item.RawValue = null;
                }
            }
            if (operation.StartsWith("del"))
            {
              
                int index = int.Parse(operation.Substring(operation.IndexOf("_") + 1));
                model.Sales.RemoveAt(index);
                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                    item.RawValue = null;
                }
            }

            if (operation == "update")
            {
                if (ModelState.IsValid)
                {


                    p.ProductName = model.ProductName;
                    p.ProductType = model.ProductType;
                    p.Price = model.Price;
                    p.MfgDate = model.MfgDate;
                    p.InStock = model.InStock;
                    if (model.Picture != null)
                    {
                        string ext = Path.GetExtension(model.Picture.FileName);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string filePath = Path.Combine(env.WebRootPath, "Pictures", f);
                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        model.Picture.CopyTo(fs);
                        fs.Close();
                        p.Picture = f;
                    }
                    db.Database.ExecuteSql($"DELETE FROM Sales WHERE ProductId={p.ProductId}"); ;
                    foreach (var s in model.Sales)
                    {
                        p.Sales.Add(new Sale { ProductId = p.ProductId, Date = s.Date, SellerName = s.SellerName, Quantity = s.Quantity });
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            ViewBag.CurrentPicture = p.Picture;
            return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var p = db.Products.FirstOrDefault(x => x.ProductId == id);
            if (p == null) return NotFound();
            db.Products.Remove(p);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
