﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoListTutorial.Models;

namespace ToDoListTutorial.Controllers {
    public class HomeController : Controller {
        private ToDoContext context;

        public HomeController(ToDoContext context) {
            this.context = context;
        }

        public IActionResult Index(string id) {
            var model = new ToDoViewModel();
            model.Filters = new Filters(id);
            model.Categories = context.Categories.ToList();
            model.Statuses = context.Statuses.ToList();
            model.DueFilters = Filters.DueFilterValues;

            //Filter model
            IQueryable<ToDo> query = context.ToDos.Include(c => c.Category).Include(s => s.Status);

            if (model.Filters.HasCategory) {
                query = query.Where(c => c.CategoryId == model.Filters.CategoryId);
            }

            if (model.Filters.HasStatus) {
                query = query.Where(s => s.StatusId == model.Filters.StatusId);
            }

            if (model.Filters.HasDue) {
                var today = DateTime.Today;
                
                if (model.Filters.IsPast) {
                    query = query.Where(d => d.DueDate < today);
                }

                if (model.Filters.IsFuture) {
                    query = query.Where(d => d.DueDate > today);
                }

                if (model.Filters.IsToday) {
                    query = query.Where(d => d.DueDate == today);
                }
            }

            var tasks = query.OrderBy(d => d.DueDate);
            model.Tasks = tasks.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult Add() {
            var model = new ToDoViewModel();
            model.Categories = context.Categories.ToList();
            model.Statuses = context.Statuses.ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ToDoViewModel model) {
            if (ModelState.IsValid) {
                context.ToDos.Add(model.CurrentTask);
                context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else {
                model.Categories = context.Categories.ToList();
                model.Statuses = context.Statuses.ToList();
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult EditDelete([FromRoute] string id, ToDo selected) {
            if (selected.StatusId == null) {
                context.ToDos.Remove(selected);
            }
            else {
                string newStatusId = selected.StatusId;
                selected = context.ToDos.Find(selected.Id);
                selected.StatusId = newStatusId;
                context.ToDos.Update(selected);
            }

            context.SaveChanges();
            return RedirectToAction("Index", "Home", new { ID = id });
        }

        [HttpPost]
        public IActionResult Filter(string[] filter) {
            string id = string.Join('-', filter);
            return RedirectToAction("Index", "Home", new { ID = id });
        }
    }
}
