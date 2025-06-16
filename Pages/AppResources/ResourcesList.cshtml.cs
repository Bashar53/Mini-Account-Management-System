using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mini_Account_Management_System.DbConnection;
using Mini_Account_Management_System.Models;
using Mini_Account_Management_System.Service;

namespace Mini_Account_Management_System.Pages.AppResouces
{

   
        public class ResourcesListModel : PageModel
        {
            private readonly ApplicationDbContext _context;
            private readonly PermissionService _permissionService;

            public ResourcesListModel(ApplicationDbContext context, PermissionService permissionService)
            {
                _context = context;
                _permissionService = permissionService;
            }

            [BindProperty]
            public AppResource Input { get; set; }

            public List<AppResource> Resources { get; set; }

            public void OnGet()
            {
                LoadResources();
            }

            public async Task<IActionResult> OnPostSaveAsync()
            {
                if (!ModelState.IsValid)
                {
                    LoadResources();
                    return Page();
                }

                var currentUrl = HttpContext.Request.Path.Value;
                var action = Input.AppResourceId == 0 ? "create" : "update";

                if (!await _permissionService.HasPermissionByUrlAsync(currentUrl, action))
                    return Forbid();

                if (Input.AppResourceId == 0)
                {
                    _context.AppResources.Add(Input);
                }
                else
                {
                    var existing = await _context.AppResources.FindAsync(Input.AppResourceId);
                    if (existing == null) return NotFound();

                    existing.ModelName = Input.ModelName;
                    existing.DisplayName = Input.DisplayName;
                    existing.Url = Input.Url;
                    existing.ParentId = Input.ParentId;
                    existing.MenuOrder = Input.MenuOrder;
                }
            TempData["SuccessMessage"] = "App Resouces saved successfully.";
            await _context.SaveChangesAsync();
                return RedirectToPage();
            }

            public async Task<IActionResult> OnPostEditAsync(int id)
            {
                var resource = await _context.AppResources.FindAsync(id);
                if (resource == null)
                    return NotFound();

                Input = new AppResource
                {
                    AppResourceId = resource.AppResourceId,
                    ModelName = resource.ModelName,
                    DisplayName = resource.DisplayName,
                    Url = resource.Url,
                    ParentId = resource.ParentId,
                    MenuOrder = resource.MenuOrder
                };

                LoadResources();
                return Page();
            }

            private void LoadResources()
            {
                Resources = _context.AppResources.ToList();
            }
        }
    }


