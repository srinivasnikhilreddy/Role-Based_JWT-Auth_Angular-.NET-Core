using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Authentication1_Core_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleBasedAuthController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("Admin")] //If HttpClient() we have to use GetAdminData() while requesting.
        public IActionResult getAdminData()
        {
            if (!User.IsInRole("Admin"))
            {
                return new JsonResult(new { message = "You don't have access to this resource." }) { StatusCode = StatusCodes.Status403Forbidden };
            }
            return Ok(new { message = "Hello, This endpoint is accessible to Admin role." });
        }

        [Authorize(Roles = "User")]
        [HttpGet("User")]
        public IActionResult getUserData()
        {
            if (!User.IsInRole("User"))
            {
                return new JsonResult(new { message = "You don't have access to this resource." }) { StatusCode = StatusCodes.Status403Forbidden };
            }
            return Ok(new { message = "Hello, This endpoint is accessible to User role." });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("Admins")]
        public IActionResult getAdminsData()
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("SuperAdmin"))
            {
                return new JsonResult(new { message = "You don't have access to this resource." }) { StatusCode = StatusCodes.Status403Forbidden };
            }
            return Ok(new { message = "Hello, This endpoint is accessible to Admin and SuperAdmin roles." });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("SuperAdmin")]
        public IActionResult getSuperAdminData()
        {
            if (!User.IsInRole("SuperAdmin"))
            {
                return new JsonResult(new { message = "You don't have access to this resource." }) { StatusCode = StatusCodes.Status403Forbidden };
            }
            return Ok(new { message = "Hello, This endpoint is accessible to SuperAdmin role." });
        }
    }
}
