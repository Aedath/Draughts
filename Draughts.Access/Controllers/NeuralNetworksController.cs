using Draughts.Access.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Draughts.Access.Controllers
{
    [Authorize]
    public class NeuralNetworksController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: api/NeuralNetworks
        [ResponseType(typeof(NeuralNetwork))]
        public async Task<IHttpActionResult> GetNeuralNetwork()
        {
            var neuralNetwork = await _db.NeuralNetworks
                .OrderByDescending(x => x.Generation)
                .FirstOrDefaultAsync();
            if (neuralNetwork == null)
            {
                return NotFound();
            }

            return Ok(neuralNetwork.ToViewModel());
        }

        // GET: api/NeuralNetworks/5
        [ResponseType(typeof(NeuralNetwork))]
        public async Task<IHttpActionResult> GetNeuralNetwork(int generation)
        {
            var neuralNetwork = await _db.NeuralNetworks
                .OrderByDescending(x => x.Generation)
                .FirstOrDefaultAsync(x => x.Generation == generation);
            if (neuralNetwork == null)
            {
                return NotFound();
            }

            return Ok(neuralNetwork.ToViewModel());
        }

        [ResponseType(typeof(int[]))]
        [Route("api/NeuralNetworks/Generations")]
        public async Task<IHttpActionResult> GetGenerations()
        {
            var generations = await _db.NeuralNetworks
                .OrderByDescending(x => x.Generation)
                .Select(x => x.Generation)
                .ToListAsync();

            return Ok(new { generations });
        }

        // PUT: api/NeuralNetworks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNeuralNetwork(int id, NeuralNetwork neuralNetwork)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != neuralNetwork.NeuralNetworkId)
            {
                return BadRequest();
            }

            _db.Entry(neuralNetwork).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NeuralNetworkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/NeuralNetworks
        [Authorize(Roles = "admin")]
        [ResponseType(typeof(NeuralNetwork))]
        public async Task<IHttpActionResult> PostNeuralNetwork(NeuralNetwork neuralNetwork)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.NeuralNetworks.Add(neuralNetwork);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = neuralNetwork.NeuralNetworkId }, neuralNetwork);
        }

        // DELETE: api/NeuralNetworks/5
        [ResponseType(typeof(NeuralNetwork))]
        public async Task<IHttpActionResult> DeleteNeuralNetwork(int id)
        {
            NeuralNetwork neuralNetwork = await _db.NeuralNetworks.FindAsync(id);
            if (neuralNetwork == null)
            {
                return NotFound();
            }

            _db.NeuralNetworks.Remove(neuralNetwork);
            await _db.SaveChangesAsync();

            return Ok(neuralNetwork);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NeuralNetworkExists(int id)
        {
            return _db.NeuralNetworks.Count(e => e.NeuralNetworkId == id) > 0;
        }
    }
}