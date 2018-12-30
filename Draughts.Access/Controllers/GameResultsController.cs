using Draughts.Access.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Draughts.Access.Controllers
{
    [Authorize]
    public class GameResultsController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: api/GameResults
        public async Task<IEnumerable<GameResultViewModel>> GetGameResults()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var results = await _db.GameResults
                            .Include(x => x.Network)
                        .Include(x => x.User)
                        .Where(x => x.User.Id == userId)
                        .Select(x => new GameResultViewModel { Generation = x.Network.Generation, Score = x.Score })
                        .ToListAsync();

                return results;
            }
            catch (System.Exception ex)
            {
                return new List<GameResultViewModel>();
            }
        }

        // POST: api/GameResults
        [ResponseType(typeof(GameResult))]
        public async Task<IHttpActionResult> PostGameResult(GameResultViewModel gameResultData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _db.Users.Find(User.Identity.GetUserId());
            var network = await _db.NeuralNetworks
                .SingleOrDefaultAsync(x => x.Generation == gameResultData.Generation);
            var gameResult = new GameResult
            {
                User = user,
                Network = network,
                Score = gameResultData.Score
            };
            _db.GameResults.Add(gameResult);
            await _db.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}