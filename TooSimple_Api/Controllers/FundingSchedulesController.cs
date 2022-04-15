using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TooSimple_Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class FundingSchedulesController : ControllerBase
	{
		public FundingSchedulesController()
		{
		}
	}
}

