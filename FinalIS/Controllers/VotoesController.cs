using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalIS.Models;
using NuGet.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace FinalIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotoesController : ControllerBase
    {
        private readonly FinalisContext _context;
        static int estado = 1;
        public static string _tokenvalidar = "";

        public VotoesController()
        {
            
        }

        [Route("Login")]
        [HttpPost]
        public Models.Token Login(User usuario)
        {
            FinalisContext _bicicletaContext = new FinalisContext();
            Models.Token tokenResult = new Models.Token();

           User usuarios = _bicicletaContext.User.Select(s =>
          new User
          {
              Id = s.Id,
              Usernombre = s.Usernombre,
              Password = s.Password,
          }
          ).FirstOrDefault(s => s.Usernombre == usuario.Usernombre);

            if (ComparePasswords(usuarios.Password, usuario.Password))
            {
                var _token = CustomTokenJWT(usuario.Usernombre);
                _tokenvalidar = _token;
                return new Models.Token
                {
                    id = usuarios.Id,
                    username = usuarios.Usernombre,
                    token = CustomTokenJWT(usuarios.Usernombre)
                };
            }

            //return StatusCode(StatusCodes.Status401Unauthorized, new { Message = "Correo o Contraseña Incorectos", Auth = Login });
            return null;

        }


        [Route("RegisterUser")]
        [HttpPost]
        public async Task<GeneralResult> RegisterUser(User usuario)
        {

            GeneralResult generalResult = new GeneralResult
            {
                Result = false
            };
            try
            {
                FinalisContext _bicicletaContext = new FinalisContext();
                usuario.Password = EncryptPassword(usuario.Password!);
                User usuario1 = new User
                {
                    Id = usuario.Id,
                    Usernombre = usuario.Usernombre,
                    Password = usuario.Password
                };
                _bicicletaContext.User.Add(usuario1);
                await _bicicletaContext.SaveChangesAsync();
                generalResult.Result = true;

            }
            catch (Exception ex)
            {
                generalResult.Result = false;
                generalResult.ErrorMessage = ex.Message;
            }
            return generalResult;
        }

        public static string EncryptPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }

        public static bool ComparePasswords(string savedPasswordHash, string password)
        {
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }


        // GET: api/Votoes
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetList")]
        [HttpGet]
        public async Task<IEnumerable<Voto>> GetList()
        {
            FinalisContext _votoContext = new FinalisContext();
            IEnumerable<Voto> votos = _votoContext.Votos.Select(s =>
            new Voto
            {
                Id = s.Id,
                IdVotante = s.IdVotante,
                IdCandidato = s.IdCandidato,
                Fecha = s.Fecha,
                EsNulo = s.EsNulo,

            }
            ).ToList();
            return votos;
        }

        //// GET: api/Votoes/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Voto>> GetVoto(int id)
        //{
        //  if (_context.Votos == null)
        //  {
        //      return NotFound();
        //  }
        //    var voto = await _context.Votos.FindAsync(id);

        //    if (voto == null)
        //    {
        //        return NotFound();
        //    }

        //    return voto;
        //}

        // PUT: api/Votoes/5
        //EN ESTE CASO NO SE PUEDE MODIFICAR UN VOTO
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutVoto(int id, Voto voto)
        //{
        //    if (id != voto.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(voto).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!VotoExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Votoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("PostVoto")]
        [HttpPost]
        public async Task<ActionResult<GeneralResult>> PostVoto(Voto voto)
        {
            GeneralResult generalResult = new GeneralResult
            {
                Result = false
            };
            if (_tokenvalidar != "")
            {
                
                if (estado == 1)
                {
                    try
                    {
                        // Consultar si el votante ya ha votado
                        FinalisContext _votoContext = new FinalisContext();
                        bool votanteYaVoto = await _votoContext.Votos.AnyAsync(v => v.IdVotante == voto.IdVotante);

                        if (votanteYaVoto)
                        {
                            generalResult.Result = false;
                            generalResult.ErrorMessage = "El votante ya ha votado y no se puede votar más de una vez.";
                            return generalResult;
                        }

                        // Agregar el voto a la base de datos y actualizar las estadísticas

                        sbyte esNulo = voto.IdCandidato == null ? (sbyte)1 : (sbyte)0; // Si IdCandidato es nulo, asignar 1 a EsNulo; de lo contrario, asignar 0.
                        Voto voto1 = new Voto
                        {
                            IdVotante = voto.IdVotante,
                            IdCandidato = voto.IdCandidato,
                            Fecha = voto.Fecha,
                            EsNulo = (sbyte)esNulo,
                            ip = voto.ip
                        };
                        _votoContext.Votos.Add(voto1);
                        await _votoContext.SaveChangesAsync();

                        generalResult.Result = true;
                    }
                    catch (Exception ex)
                    {
                        generalResult.Result = false;
                        generalResult.ErrorMessage = ex.Message;
                    }
                    return generalResult;
                }
                else
                {
                    generalResult.Result = false;
                    generalResult.ErrorMessage = "Se Cerro la fase";
                    return BadRequest(generalResult);
                }
            }
            else
            {
                generalResult.Result = false;
                generalResult.ErrorMessage = "Se Cerro la fase";
                return BadRequest(generalResult);
            }
            
        }

        private string CustomTokenJWT(string username)
        {
            IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
            JWTResult jWTResult = config.GetRequiredSection("JWT").Get<JWTResult>();

            var _symmetricSecurityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jWTResult.SecretKey)
                );
            var _signingCredentials = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            var _Header = new JwtHeader(_signingCredentials);
            var _Claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name, username),
                new Claim("Name", "nombrepersona")
            };
            var _Payload = new JwtPayload(
                    issuer: jWTResult.Issuer,
                    audience: jWTResult.Audience,
                    claims: _Claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddHours(2)
                );
            var _Token = new JwtSecurityToken(
                    _Header,
                    _Payload
                );
            return new JwtSecurityTokenHandler().WriteToken(_Token);
        }



        // DELETE: api/Votoes/5
        //TAMPOCO SE PODRÁ ELIMINAR UN VOTO
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteVoto(int id)
        //{
        //    if (_context.Votos == null)
        //    {
        //        return NotFound();
        //    }
        //    var voto = await _context.Votos.FindAsync(id);
        //    if (voto == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Votos.Remove(voto);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool VotoExists(int id)
        {
            return (_context.Votos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
