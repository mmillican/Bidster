using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bidster.Data;
using Bidster.Entities.Products;
using Bidster.Entities.Users;
using Bidster.Models;
using Bidster.Models.Products;
using Bidster.Services.FileStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bidster.Controllers
{
    [Route("events/{evtSlug}/products")]
    public class ProductsController : Controller
    {
        // 0 = event slug --- 1 == filename
        private const string ImagePathFormat = "events/{0}/products/{1}";

        private readonly BidsterDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IFileService _fileService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(BidsterDbContext dbContext,
            UserManager<User> userManager,
            IFileService fileService,
            ILogger<ProductsController> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _fileService = fileService;
            _logger = logger;
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> Details(string evtSlug, string slug)
        {
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == evtSlug);
            if (evt == null)
            {
                _logger.LogInformation("Event slug '{evtSlug}' not found", evtSlug);
                return RedirectToAction("Index", "Events");
            }

            var product = await _dbContext.Products.SingleOrDefaultAsync(x => x.EventId == evt.Id && x.Slug == slug);
            if (product == null)
            {
                _logger.LogInformation("Product slug '{slug}' not found or does not belong to event '{evtSlug}'", slug, evtSlug);
                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }

            var user = await _userManager.GetUserAsync(User);

            var model = new ProductDetailsViewModel();
            model.Event = ModelMapper.ToEventModel(evt);
            model.Product = ModelMapper.ToProductModel(product);
            
            if (!string.IsNullOrEmpty(product.ImageFilename))
            {
                model.Product.ImageUrl = _fileService.ResolveFileUrl(string.Format(ImagePathFormat, evt.Slug, product.ImageFilename));
            }

            model.CanUserEdit = user != null && user.Id == evt.OwnerId;

            model.Bids = await _dbContext.Bids
                .Include(x => x.User)
                .Where(x => x.ProductId == product.Id)
                .OrderByDescending(x => x.Timestamp)
                .Select(x => x.ToBidModel())
                .ToListAsync();

            return View(model);
        }

        [Authorize]
        [HttpGet("new")]
        public async Task<IActionResult> Create(string evtSlug)
        {
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == evtSlug);
            var user = await _userManager.GetUserAsync(User);

            if (evt == null)
            {
                _logger.LogInformation("Could not find event with slug '{slug}'", evtSlug);
                return RedirectToAction("Index", "Events");
            }

            if (evt.OwnerId != user.Id)
            {
                _logger.LogInformation("Cannot edit event '{slug}' because user ID '{userId}' is not the owner", evtSlug, user.Id);
                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }

            var model = new EditProductViewModel
            {
                EventId = evt.Id,
                EventSlug = evt.Slug,
                EventName = evt.Name
            };

            return View(model);
        }

        [Authorize]
        [HttpPost("new")]
        public async Task<IActionResult> Create(string evtSlug, EditProductViewModel model)
        {
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == evtSlug);
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (evt == null)
            {
                _logger.LogInformation("Could not find event with slug '{slug}'", evtSlug);
                return RedirectToAction("Index", "Events");
            }

            if (evt.OwnerId != user.Id)
            {
                _logger.LogInformation("Cannot edit event '{slug}' because user ID '{userId}' is not the owner", evtSlug, user.Id);
                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }

            // Reset event props in case something fails...
            model.EventId = evt.Id;
            model.EventSlug = evt.Slug;
            model.EventName = evt.Name;

            try
            {
                var product = new Product
                {
                    Event = evt,
                    Name = model.Name,
                    Description = model.Description,
                    StartingPrice = model.StartingPrice,
                    MinimumBidAmount = model.MinimumBidAmount,
                    CurrentBidAmount = model.StartingPrice
                };

                product.Slug = await GenerateSlugAsync(evt.Id, product.Name);

                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();

                // Save image after the product is created because we need the ID
                if (model.ImageFile != null)
                {
                    product.ImageFilename = GenerateImageFilename(product, model.ImageFile.FileName);

                    var imagePath = string.Format(ImagePathFormat, evt.Slug, product.ImageFilename);

                    using (var stream = model.ImageFile.OpenReadStream())
                    {
                        await _fileService.SaveFileAsync(imagePath, model.ImageFile.ContentType, stream);
                    }

                    _dbContext.Products.Update(product);
                    await _dbContext.SaveChangesAsync();
                }

                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product '{name}' to event ID {evtId}", model.Name, evt.Id);

                return View(model);
            }
        }

        [Authorize]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string evtSlug, int id)
        {
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == evtSlug);
            var user = await _userManager.GetUserAsync(User);

            if (evt == null)
            {
                _logger.LogInformation("Could not find event with slug '{slug}'", evtSlug);
                return RedirectToAction("Index", "Events");
            }

            if (evt.OwnerId != user.Id)
            {
                _logger.LogInformation("Cannot edit event '{slug}' because user ID '{userId}' is not the owner", evtSlug, user.Id);
                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null || product.EventId != evt.Id)
            {
                _logger.LogInformation("Product ID {id} not found", id);
                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }

            var model = new EditProductViewModel
            {
                Id = id,

                EventId = evt.Id,
                EventSlug = evt.Slug,
                EventName = evt.Name,

                Name = product.Name,
                Description = product.Description,
                StartingPrice = product.StartingPrice,
                MinimumBidAmount = product.MinimumBidAmount,
                HasBids = product.HasBids,

                ImageFilename  = product.ImageFilename
            };

            if (!string.IsNullOrEmpty(model.ImageFilename))
            {
                var path = string.Format(ImagePathFormat, evt.Slug, product.ImageFilename);
                model.ImageUrl = _fileService.ResolveFileUrl(path);
            }

            return View(model);
        }

        [Authorize]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(string evtSlug, int id, EditProductViewModel model)
        {
            var evt = await _dbContext.Events.SingleOrDefaultAsync(x => x.Slug == evtSlug);
            var user = await _userManager.GetUserAsync(User);

            if (evt == null)
            {
                _logger.LogInformation("Could not find event with slug '{slug}'", evtSlug);
                return RedirectToAction("Index", "Events");
            }

            if (evt.OwnerId != user.Id)
            {
                _logger.LogInformation("Cannot edit event '{slug}' because user ID '{userId}' is not the owner", evtSlug, user.Id);
                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }

            // Reset event props in case something fails...
            model.Id = id;
            model.EventId = evt.Id;
            model.EventSlug = evt.Slug;
            model.EventName = evt.Name;

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null || product.EventId != evt.Id)
            {
                _logger.LogInformation("Product ID {id} not found", id);
                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }

            try
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.StartingPrice = model.StartingPrice;
                product.MinimumBidAmount = model.MinimumBidAmount;

                if (model.ImageFile != null)
                {
                    product.ImageFilename = GenerateImageFilename(product, model.ImageFile.FileName);

                    var imagePath = string.Format(ImagePathFormat, evt.Slug, product.ImageFilename);

                    using (var stream = model.ImageFile.OpenReadStream())
                    {
                        await _fileService.SaveFileAsync(imagePath, model.ImageFile.ContentType, stream);
                    }
                }

                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Details", "Events", new { slug = evtSlug });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating product ID {id}", id);

                return View(model);
            }
        }

        private async Task<string> GenerateSlugAsync(int eventId, string name)
        {
            var slug = name.Clean();

            if (!await DoesSlugExist(eventId, slug))
            {
                return slug;
            }
        
            var appendIdx = 1;
            var modSlug = $"{slug}-{appendIdx})";
            while (await DoesSlugExist(eventId, modSlug))
            {
                appendIdx++;
                modSlug = $"{slug}-{appendIdx})";
            }

            return modSlug;
        }

        private async Task<bool> DoesSlugExist(int eventId, string slug) =>
            await _dbContext.Products.AnyAsync(x => x.EventId == eventId && x.Slug == slug);

        private static string GenerateImageFilename(Product product, string origFilename)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            if (string.IsNullOrEmpty(origFilename))
            {
                throw new ArgumentException("message", nameof(origFilename));
            }

            var fileExt = Path.GetExtension(origFilename);

            var sluggedProductName = product.Slug.Truncate(90);
            var filename = $"{product.Id}-{sluggedProductName}{fileExt}";

            return filename;
        }
    }
}