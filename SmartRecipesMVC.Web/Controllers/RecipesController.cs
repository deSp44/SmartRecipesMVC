﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SmartRecipesMVC.Application.Interfaces;
using SmartRecipesMVC.Application.ViewModels.IngredientVm;
using SmartRecipesMVC.Application.ViewModels.RecipeVm;
using SmartRecipesMVC.Domain.Model;
using SmartRecipesMVC.Domain.Model.Connections;

namespace SmartRecipesMVC.Web.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        public string AuthenticateUser()
        {
            string _userId;
            if (User.Identity is ClaimsIdentity claimsIdentity)
            {
                var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                _userId = userIdClaim.Value;
                return _userId;
            }
            return null;
        }

        [HttpGet] public IActionResult Index()
        {
            var userId = AuthenticateUser();
            var model = _recipeService.GetAllRecipesForList(12, 1, "", false, userId);
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost] public IActionResult Index(int pageSize, int? pageNumber, string searchString)
        {
            var userId = AuthenticateUser();
            pageNumber ??= 1;
            searchString ??= string.Empty;
        
            var model = _recipeService.GetAllRecipesForList(pageSize, pageNumber.Value, searchString, false, userId);
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
        [HttpPost] public IActionResult AddRecipe([Bind("Name,Description,CreateDate,PreparationTime,Portions,Preparation,Hints,DifficultyId,IsActive,RecipeIngredients")]NewRecipeVm model)
        {
            var id = _recipeService.AddRecipe(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrderItem([Bind("RecipeIngredients")] NewRecipeVm newRecipeVm)
        {
            newRecipeVm.RecipeIngredients.Add(new RecipeIngredient());
            return PartialView("RecipeIngredients", newRecipeVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveOrderItem([Bind("RecipeIngredients")] NewRecipeVm newRecipeVm)
        {
            newRecipeVm.RecipeIngredients.RemoveAt(newRecipeVm.RecipeIngredients.Count - 1);
            return PartialView("RecipeIngredients", newRecipeVm);
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

        public IActionResult MoveToTrash(int id)
        {
            _recipeService.MoveToTrash(id);
            return RedirectToAction("Index");
        }

    }
}
