using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalIS.Models;
using Microsoft.AspNetCore.Mvc;


namespace FinalIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatoesController : ControllerBase
    {
        private readonly FinalisContext _context;
        static int estado = 1;
       

        public CandidatoesController()
        {

        }

        // GET: api/Candidatoes
        [Route("GetList")]
        [HttpGet]
        public async Task<IEnumerable<Candidato>> GetList()
        {
            FinalisContext _candidatoContext = new FinalisContext();
            IEnumerable<Candidato> candidatos = _candidatoContext.Candidatos.Select(s =>
            new Candidato
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Partido = s.Partido,
                Descripcion = s.Descripcion,
            }
            ).ToList();
            return candidatos;
        }

        // GET: api/Candidatoes/5
        [Route("GetCandidato")]
        [HttpGet]
        public Candidato GetCandidato(int id)
        {
            FinalisContext _candidatoContext = new FinalisContext();
            Candidato candidatos = _candidatoContext.Candidatos.Select(s =>
            new Candidato
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Partido = s.Partido,
                Descripcion = s.Descripcion,
            }
            ).FirstOrDefault(s => s.Id == id);
            return candidatos;
        }

        // PUT: api/Candidatoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("PutCandidato")]
        [HttpPut]
        public async Task<GeneralResult> PutCandidato(Candidato _candidato)
        {
            GeneralResult generalResult = new GeneralResult
            {
                Result = false
            };
            try
            {
                FinalisContext _candidatoContext = new FinalisContext();
                Candidato candidato = new Candidato
                {
                    Id = _candidato.Id,
                    Nombre = _candidato.Nombre,
                    Partido = _candidato.Partido,
                    Descripcion = _candidato.Descripcion,
                };
                _candidatoContext.Update(candidato);
                await _candidatoContext.SaveChangesAsync();
                generalResult.Result = true;
            }
            catch (Exception ex)
            {
                generalResult.Result = false;
                generalResult.ErrorMessage = ex.Message;
            }
            return generalResult;
        }

        // POST: api/Candidatoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("PostCandidato")]
        [HttpPost]
        public async Task<ActionResult<GeneralResult>> PostCandidato(Candidato candidato)
        {
            GeneralResult generalResult = new GeneralResult
            {
                Result = false
            };
            if (estado == 1)
            {
                try
                {
                    FinalisContext _candidatoContext = new FinalisContext();
                    Candidato candidatos1 = new Candidato
                    {
                        Id = candidato.Id,
                        Nombre = candidato.Nombre,
                        Partido = candidato.Partido,
                        Descripcion = candidato.Descripcion,
                    };
                    _candidatoContext.Candidatos.Add(candidatos1);
                    await _candidatoContext.SaveChangesAsync();
                    generalResult.Result = true;
                    estado = 0;
                }
                catch (Exception ex)
                {
                    generalResult.Result = false;
                    generalResult.ErrorMessage = ex.Message;
                }
                return Ok(generalResult);
            }
            else
            {
                generalResult.Result = false;
                generalResult.ErrorMessage = "Se Cerro la fase";
                return BadRequest(generalResult);
            }
        }




        // DELETE: api/Candidatoes/5
        [Route("DeleteCandidato")]
        [HttpDelete]
        public async Task<GeneralResult> DeleteCandidato(int id)
        {
            GeneralResult generalResult = new GeneralResult
            {
                Result = false
            };
            try
            {
                FinalisContext _candidatoContext = new FinalisContext();
                var candidatoToDelete = await _candidatoContext.Candidatos.FindAsync(id);
                if (candidatoToDelete == null)
                {
                    generalResult.Result = false;
                    generalResult.ErrorMessage = "Candidato not found.";
                    return generalResult;
                }
                _candidatoContext.Candidatos.Remove(candidatoToDelete);
                await _candidatoContext.SaveChangesAsync();
                generalResult.Result = true;
            }
            catch (Exception ex)
            {
                generalResult.Result = false;
                generalResult.ErrorMessage = ex.Message;
            }
            return generalResult;
        }

        private bool CandidatoExists(int id)
        {
            return (_context.Candidatos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
