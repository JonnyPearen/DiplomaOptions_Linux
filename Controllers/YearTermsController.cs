﻿﻿using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiplomaWebSite.Models;
using DiplomaWebSite.Services;
//using testyo2.ViewModels.Manage;
using DiplomaWebSite;

namespace DiplomaWebSite.Controllers
{
        //[Authorize(Roles = "Admin")]
    public class YearTermsController : Controller
    {
        private OptionsContext _context { get; set; }

        [FromServices]
        public ILogger<YearTermsController> Logger { get; set; }

        public YearTermsController(OptionsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.YearTerms.ToList());
        }

        public ActionResult Create()
        {
       //     ViewBag.Items = GetYearTermsListItems();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(YearTerm yearTerm)
        {
            int check =  yearTerm.term;
            
            if (check!=10 && check!=20 && check!= 30) {
                 ModelState.AddModelError(string.Empty, "Valid terms are 10, 20, and 30.");
            }
           
            if (ModelState.IsValid)
            {
                
                if (yearTerm.isDefault)
                {
                    var query = from year in _context.YearTerms where year.isDefault == true select year;
                    foreach (YearTerm year in query)
                    {
                        year.isDefault = false;
                    }
                }
                _context.YearTerms.Add(yearTerm);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(yearTerm);
        }

        public ActionResult Details(int id)
        {
            YearTerm yearTerm = _context.YearTerms
                .Where(b => b.YearTermId == id)
                .FirstOrDefault();
            if (yearTerm == null)
            {
                Logger.LogInformation("Details: Item not found {0}", id);
                return HttpNotFound();
            }
            return View(yearTerm);
        }
/*
        private IEnumerable<SelectListItem> GetYearTermsListItems(int selected = -1)
        {
            var tmp = _context.YearTerms.ToList();

            // Create authors list for <select> dropdown
            return tmp
                .OrderBy(s => s.title)
                .Select(s => new SelectListItem
                {
                    Text = String.Format("{0}, {1}", s.YearTermId, s.title),
                    Value = s.YearTermId.ToString(),
                    Selected = s.YearTermId == selected
                });
        }
    */

        public async Task<ActionResult> Edit(int id)
        {
            YearTerm yearTerm = await FindYearTermAsync(id);
            if (yearTerm == null)
            {
                Logger.LogInformation("Edit: Item not found {0}", id);
                return HttpNotFound();
            }

         //   ViewBag.Items = GetYearTermsListItems(YearTerm.YearTermId);
            return View(yearTerm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, YearTerm yearTerm)
        {
            try
            {
                if (yearTerm.isDefault)
                {
                    var query = from year in _context.YearTerms where year.isDefault == true && year.YearTermId != yearTerm.YearTermId select year;
                    foreach (YearTerm year in query)
                    {
                        year.isDefault = false;
                    }
                }
                yearTerm.YearTermId = id;
                _context.YearTerms.Attach(yearTerm);
                _context.Entry(yearTerm).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes.");
            }
            return View(yearTerm);
        }

        private Task<YearTerm> FindYearTermAsync(int id)
        {
            return _context.YearTerms.SingleOrDefaultAsync(s => s.YearTermId == id);
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<ActionResult> ConfirmDelete(int id, bool? retry)
        {
            YearTerm yearTerm = await FindYearTermAsync(id);
            if (yearTerm == null)
            {
                Logger.LogInformation("Delete: Item not found {0}", id);
                return HttpNotFound();
            }
            ViewBag.Retry = retry ?? false;
            return View(yearTerm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                YearTerm yearTerm = await FindYearTermAsync(id);
                _context.YearTerms.Remove(yearTerm);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Delete", new { id = id, retry = true });
            }
            return RedirectToAction("Index");
        }
    }
}
