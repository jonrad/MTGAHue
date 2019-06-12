using Microsoft.AspNetCore.Mvc;
using MTGADispatcher;
using MTGADispatcher.Events;
using System;

namespace MagicLights.Api.Controllers
{
    [Route("api/[controller]")]
    public class DemoController : Controller
    {
        private readonly Game game;

        public DemoController(Game game)
        {
            this.game = game;
        }

        [HttpPost("cast_spell")]
        public void CastSpell(MagicColor[]? colors)
        {
            if (colors == null)
            {
                throw new ArgumentException();
            }

            game.Events.Dispatch<CastSpell>(new CastSpell(
                new Instance(1, 1, 1, colors)));
        }
    }
}
