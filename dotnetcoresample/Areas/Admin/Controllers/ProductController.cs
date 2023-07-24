using Dotnet.DataAccess.Repository;
using Dotnet.DataAccess.Repository.IRepository;
using Dotnet.Models;
using Dotnet.Models.ViewModels;
using Dotnet.Utility;
using dotnetcoresample.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Data;

namespace dotnetcoresample.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;   
        }

        //retreiving data 
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
        }

        //get action method
        public IActionResult Upsert(int? id)
        {

            //ViewBag.CategoryList = CategoryList;    

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                }),
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

            
        }

        [HttpPost]
        public IActionResult Upsert (ProductVM productVM, IFormFile? file)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The Display Order cannot Exactly the same Display Name");
            //}
            //if (obj.Name != null && obj.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("name", "Test is an invalid value");//error message near name tag
            //}
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    
                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                
                _unitOfWork.Save();
                TempData["success"] = "Product Added Succesfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
                return View(productVM);
            }
           
            

        }

        //Delete method
       
        //reteiving data == _db.category
     

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductList});   
        }

        [HttpDelete] 
        public IActionResult Delete(int? id)
        {
            var productToBeDeletedn = _unitOfWork.Product.Get(u=>u.Id == id);
            if(productToBeDeletedn == null)
            {
                return Json(new {success= false, message="Error while deleting"});
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeletedn.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeletedn);
            _unitOfWork.Save(); 
            return Json(new { success = true, message = "Delete Successfully" });
        }
        #endregion
    }
}
