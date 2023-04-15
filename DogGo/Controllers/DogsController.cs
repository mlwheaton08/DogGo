using DogGo.Models;
using DogGo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class DogsController : Controller
    {
        private readonly IDogRepository _dogRepo;

        public DogsController(IDogRepository dogRepo)
        {
            _dogRepo = dogRepo;
        }

        // GET: DogsController
        [Authorize]
        public ActionResult Index()
        {
            int ownerId = GetCurrentUserId();

            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);

            return View(dogs);
        }

        // GET: DogsController/Details/5
        public ActionResult Details(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);

            if(dog == null)
            {
                return NotFound();
            }

            return View(dog);
        }

        // GET: DogsController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: DogsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog dog)
        {
            try
            {
                dog.OwnerId = GetCurrentUserId();
                _dogRepo.AddDog(dog);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(dog);
            }
        }

        // GET: DogsController/Edit/5
        public ActionResult Edit(int id)
        {
            int ownerId = GetCurrentUserId();

            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);
            var validDog = dogs.FirstOrDefault(d => d.Id == id);

            if(validDog == null)
            {
                return NotFound();
            }
            else
            {
                Dog dog = _dogRepo.GetDogById(id);
                return View(dog);
            }

        }

        // POST: DogsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog dog)
        {
            try
            {
                dog.OwnerId = GetCurrentUserId();
                _dogRepo.UpdateDog(dog);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(dog);
            }
        }

        // GET: DogsController/Delete/5
        public ActionResult Delete(int id)
        {
            int ownerId = GetCurrentUserId();

            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);
            var validDog = dogs.FirstOrDefault(d => d.Id == id);

            if (validDog == null)
            {
                return NotFound();
            }
            else
            {
                Dog dog = _dogRepo.GetDogById(id);
                return View(dog);
            }
        }

        // POST: DogsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Dog dog)
        {
            try
            {
                _dogRepo.DeleteDog(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(dog);
            }
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(id);
            }
        }
    }
}