using System;
using System.Configuration;
using System.DirectoryServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GISRZ.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace GISRZ.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		public AccountController()
			: this(new UserManager<user>(new UserStore()))
		{
		}
		
		public AccountController(UserManager<user> userManager)
		{
			UserManager = userManager;
		}

		public UserManager<user> UserManager
		{
			get;
			private set;
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, change to shouldLockout: true
			var domain = ConfigurationManager.ConnectionStrings["ADConnectionString"].ConnectionString;

			var currentUser = await UserManager.FindByNameAsync(model.Username);
			if (currentUser == null)
			{
				var user = new user
				{
					UserName = model.Username
				};

				try
				{
					var entry = new DirectoryEntry(domain, model.Username, model.Password, AuthenticationTypes.Secure);
					var search = new DirectorySearcher(entry);
					var searchResult = default(SearchResultCollection);
					search.Filter = ("(&(objectClass=user)(samaccountname=" + model.Username + "))");
					search.PropertiesToLoad.Add("givenName");
					search.PropertiesToLoad.Add("givenName"); // first name
					search.PropertiesToLoad.Add("sn"); // last name
					search.PropertiesToLoad.Add("mail"); // smtp mail address
					searchResult = search.FindAll();
					switch ((searchResult.Count))
					{
						case 1:
							foreach (SearchResult iresult in searchResult)
							{
								if (iresult.Properties.PropertyNames == null)
									continue;
								foreach (string item in iresult.Properties.PropertyNames)
								{
									foreach (var key in iresult.GetDirectoryEntry().Properties[item])
									{
										try
										{
											switch (item.ToUpper())
											{
												case "SAMACCOUNTNAME":
													user.UserName = key.ToString();
													break;

												case "GIVENNAME":
													user.first_name = key.ToString();
													break;
												case "SN":
													user.last_name = key.ToString();
													break;
												case "EMPLOYEEID":
													user.employee_id = Convert.ToInt32(key);
													break;
											}
										}
										catch (Exception ex)
										{
											Console.WriteLine(ex.Message);
										}
									}
								}
							}
							user.SecurityStamp = new Guid().ToString();
							var add = await UserManager.CreateAsync(user);
							if (add.Succeeded)
							{
								await SignInAsync(user, true);
								return RedirectToLocal(returnUrl);
							}
							break;

						case 0:
						default:
							ModelState.AddModelError("", "Invalid login attempt.");
							return View(model);
					}
				}
				catch (Exception)
				{
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
				}
			}
			else
			{
				currentUser.SecurityStamp = new Guid().ToString();
				await SignInAsync(currentUser, true);
			}
			return RedirectToLocal(returnUrl);
		}

		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private async Task SignInAsync(user user, bool isPersistent)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
			var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
			AuthenticationManager.SignIn(new AuthenticationProperties()
			{
				IsPersistent = isPersistent
			}, identity);
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		#endregion
	}
}