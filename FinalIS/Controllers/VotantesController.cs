using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalIS.Models;

namespace FinalIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotantesController : ControllerBase
    {
        private readonly FinalisContext _context;
        GeneralResult estado = new GeneralResult();

        public VotantesController()
        {
            estado.EstadoCandidato = 1;
        }

        // GET: api/Votantes
        [Route("GetList")]
        [HttpGet]
        public async Task<IEnumerable<Votante>> GetList()
        {
            FinalisContext _votanteContext = new FinalisContext();
            IEnumerable<Votante> votantes = _votanteContext.Votantes.Select(s =>
            new Votante
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Dpi = s.Dpi,
            }
            ).ToList();
            return votantes;
        }

        // GET: api/Votantes/5
        [Route("GetVotante")]
        [HttpGet]
        public Votante GetVotante(int id)
        {
            FinalisContext _votanteContext = new FinalisContext();
            Votante votantes = _votanteContext.Votantes.Select(s =>
            new Votante
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Dpi = s.Dpi,
            }
            ).FirstOrDefault(s => s.Id == id);
            return votantes;
        }

        // PUT: api/Votantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("PutVotante")]
        [HttpPut]
        public async Task<GeneralResult> PutVotante(Votante _votante)
        {
            GeneralResult generalResult = new GeneralResult
            {
                Result = false
            };
            try
            {
                FinalisContext _votanteContext = new FinalisContext();
                Votante votante = new Votante
                {
                    Id = _votante.Id,
                    Nombre = _votante.Nombre,
                    Dpi = _votante.Dpi,
                };
                _votanteContext.Update(votante);
                await _votanteContext.SaveChangesAsync();
                generalResult.Result = true;
            }
            catch (Exception ex)
            {
                generalResult.Result = false;
                generalResult.ErrorMessage = ex.Message;
            }
            return generalResult;
        }

        // POST: api/Votantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("PostVotante")]
        [HttpPost]
        public async Task<GeneralResult> PostVotante(Votante votante)
        {
            GeneralResult generalResult = new GeneralResult
            {
                Result = false
            };
            try
            {
                FinalisContext _votanteContext = new FinalisContext();
                Votante votante1 = new Votante
                {
                    Id = votante.Id,
                    Nombre = votante.Nombre,
                    Dpi = votante.Dpi,
                };
                _votanteContext.Votantes.Add(votante1);
                await _votanteContext.SaveChangesAsync();
                generalResult.Result = true;
                estado.EstadoCandidato = 0;
            }
            catch (Exception ex)
            {
                generalResult.Result = false;
                generalResult.ErrorMessage = ex.Message;
            }
            return generalResult;
        }

        // DELETE: api/Votantes/5
        [Route("DeleteVotante")]
        [HttpDelete]
        public async Task<GeneralResult> DeleteVotante(int id)
        {
            GeneralResult generalResult = new GeneralResult
            {
                Result = false
            };
            try
            {
                FinalisContext _votanteContext = new FinalisContext();
                var votanteToDelete = await _votanteContext.Votantes.FindAsync(id);
                if (votanteToDelete == null)
                {
                    generalResult.Result = false;
                    generalResult.ErrorMessage = "Votante not found.";
                    return generalResult;
                }
                _votanteContext.Votantes.Remove(votanteToDelete);
                await _votanteContext.SaveChangesAsync();
                generalResult.Result = true;
            }
            catch (Exception ex)
            {
                generalResult.Result = false;
                generalResult.ErrorMessage = ex.Message;
            }
            return generalResult;
        }
        private bool VotanteExists(int id)
        {
            return (_context.Votantes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
