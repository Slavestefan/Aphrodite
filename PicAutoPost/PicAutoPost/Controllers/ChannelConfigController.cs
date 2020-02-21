

using System;
using Microsoft.AspNetCore.Mvc;
using Slavestefan.Aphrodite.Model;

namespace Slavestefan.Aphrodite.Web.Controllers
{
    public class ChannelConfigController : Controller
    {
        private readonly PicAutoPostContext _context;

        public ChannelConfigController(PicAutoPostContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            throw new NotImplementedException();
        }
    }
}