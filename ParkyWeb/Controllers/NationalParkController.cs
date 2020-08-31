using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class NationalParkController : Controller
    {
        private readonly INationalParkRepository _repo;

        public NationalParkController(INationalParkRepository repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() {});
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();
            if (id == null) // this will be true for insert or create
            {
                return View(obj);
            }

            // Flow will come here for update
            obj = await _repo.GetAsync(SD.NationalParkApiPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    obj.Picture = p1;
                }
                else
                {
                    var objFromDb = await _repo.GetAsync(SD.NationalParkApiPath,obj.Id, HttpContext.Session.GetString("JWToken"));
                    obj.Picture = objFromDb.Picture;
                }

                if (obj.Id == 0)
                {
                    await _repo.CreateAsync(SD.NationalParkApiPath, obj, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _repo.UpdateAsync(SD.NationalParkApiPath+obj.Id, obj, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new {data = await _repo.GetAllAsync(SD.NationalParkApiPath, HttpContext.Session.GetString("JWToken")) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _repo.DeleteAsync(SD.NationalParkApiPath, id, HttpContext.Session.GetString("JWToken"));
            if (status)
            {
                return Json(new {success = true, message = "Delete Successful"});
            }

            return Json(new { success = true, message = "Delete Not Successful" });
        }
    }
}
