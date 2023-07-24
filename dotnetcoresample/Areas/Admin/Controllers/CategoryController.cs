using Dotnet.DataAccess.Repository;
using Dotnet.DataAccess.Repository.IRepository;
using Dotnet.Models;
using Dotnet.Utility;
using dotnetcoresample.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetcoresample.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //retreiving data 
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();

            return View(objCategoryList);
        }

        //get action method
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot Exactly the same Display Name");
            }
            //if (obj.Name != null && obj.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("name", "Test is an invalid value");//error message near name tag
            //}
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Added Succesfully";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //reteiving data == _db.category
        [HttpPost]
        public IActionResult Edit(Category obj)
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
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category Edited Succesfully";
                return RedirectToAction("Index");
            }

            return View();

        }

        //Delete method
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //reteiving data == _db.category
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The Display Order cannot Exactly the same Display Name");
            //}
            //if (obj.Name != null && obj.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("name", "Test is an invalid value");//error message near name tag
            //}
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Succesfully";
            return RedirectToAction("Index");

        }
    }
}
