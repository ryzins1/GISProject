using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;

namespace GISRZ.Infrastructure
{
	public static class HtmlHelpers
	{
		public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
		{
			var manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			return new MvcHtmlString(manager.FindByIdAsync(id).Result.UserName);
		}
	}
}
