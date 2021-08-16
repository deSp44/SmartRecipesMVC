﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipesMVC.Application.Interfaces;
using SmartRecipesMVC.Application.ViewModels.RecipeVm;

namespace SmartRecipesMVC.Web.Controllers
{
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet] public IActionResult Index()
        {
            var model = _recipeService.GetAllRecipesForList(12, 1, "");
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost] public IActionResult Index(int pageSize, int? pageNumber, string searchString)
        {
            pageNumber ??= 1;
            searchString ??= string.Empty;

            var model = _recipeService.GetAllRecipesForList(pageSize, pageNumber.Value, searchString);
            return View(model);
        }

        public IActionResult ViewRecipe(int recipeId)
        {
            var recipeModel = _recipeService.GetRecipeDetails(recipeId);
            return View(recipeModel);
        }

        [HttpGet] public IActionResult AddRecipe()
        {
            return View(new NewRecipeVm());
        }

        [ValidateAntiForgeryToken]
        [HttpPost] public IActionResult AddRecipe(NewRecipeVm model)
        {
            var id = _recipeService.AddRecipe(model);
            return RedirectToAction("Index");
        }

        [HttpGet] public IActionResult EditRecipe(int id)
        {
            var customer = _recipeService.GetRecipeForEdit(id);
            return View(customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost] public IActionResult EditRecipe(NewRecipeVm model)
        {
            _recipeService.UpdateRecipe(model);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteRecipe(int id)
        {
            _recipeService.DeleteRecipe(id);
            return RedirectToAction("Index");
        }
    }
}
