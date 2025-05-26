using APIRESTfulNUXIBA.Context;
using APIRESTfulNUXIBA.DTOs;
using APIRESTfulNUXIBA.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace APIRESTfulNUXIBA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDBContext _appDbcontext;

        public LoginController(AppDBContext appDbcontext)
        {
            _appDbcontext = appDbcontext;
        }

        // GET: api/Login
        [HttpGet]
        [Route("logins")]
        public async Task<ActionResult<List<LoginDTO>>> GetLogins()
        {
            var loginsDTO = new List<LoginDTO>();
            foreach (var item in await _appDbcontext.ccloglogin.ToListAsync())
            {
                loginsDTO.Add(new LoginDTO {logId = item.logId, User_id = item.User_id, Extension = item.Extension, TipoMov = item.TipoMov, fecha = item.fecha });
            }
            return Ok(loginsDTO);
        }

        // GET: api/Login/5
        [HttpGet]
        [Route("login/{id:int}")]
        public async Task<ActionResult<List<LoginDTO>>> GetLogin(int id)
        {
            var logins = await _appDbcontext.ccloglogin
                .Where(e => e.User_id == id)
                .ToListAsync();

            if (logins == null || logins.Count == 0)
            {
                return NotFound();
            }

            // Asumiendo que tienes un método para mapear a DTO, por ejemplo:
            var loginDTOs = logins.Select(l => new LoginDTO
            {
                logId = l.logId,
                User_id = l.User_id,
                Extension = l.Extension,
                TipoMov = l.TipoMov,
                fecha = l.fecha
            }).ToList();

            return Ok(loginDTOs);
        }

        // POST: api/Login
        [HttpPost]
        [Route("agregar")]
        public async Task<ActionResult<LoginDTO>> NuevoLogin([FromBody] LoginDTO loginDto)
        {
            // Validar existencia del usuario en ccUsers
            bool userExists = await _appDbcontext.ccUsers.AnyAsync(u => u.User_id == loginDto.User_id);
            if (!userExists)
            {
                return BadRequest("El usuario no existe.");
            }

            // Validar fecha
            if (loginDto.fecha == default)
            {
                return BadRequest("Fecha inválida.");
            }

            // Validar que no exista un Login sin Logout previo
            if (loginDto.TipoMov == 1)
            {
                bool hasUnclosedLogin = await _appDbcontext.ccloglogin
                    .Where(l => l.User_id == loginDto.User_id)
                    .OrderByDescending(l => l.fecha)
                    .AnyAsync(l => l.TipoMov == 1);
                bool hasLogoutAfter = await _appDbcontext.ccloglogin
                    .Where(l => l.User_id == loginDto.User_id && l.fecha > loginDto.fecha && l.TipoMov == 0)
                    .AnyAsync();

                if (hasUnclosedLogin && !hasLogoutAfter)
                {
                    return BadRequest("No se puede registrar un nuevo login sin un logout previo.");
                }
            }

            var login = new Login
            {
                logId = loginDto.logId,
                User_id = loginDto.User_id,
                Extension = loginDto.Extension,
                TipoMov = loginDto.TipoMov,
                fecha = loginDto.fecha
            };

            await _appDbcontext.ccloglogin.AddAsync(login);
            await _appDbcontext.SaveChangesAsync();

            var loginCreado = new LoginDTO
            {
                logId = login.logId,
                User_id = login.User_id,
                Extension = login.Extension,
                TipoMov = login.TipoMov,
                fecha = login.fecha
            };

            return CreatedAtAction(nameof(GetLogin), new { id = loginCreado.User_id }, loginCreado);
        }

        [HttpPut]
        [Route("editar/{logId:int}")]
        public async Task<ActionResult<LoginDTO>> EditarLogin(int logId, [FromBody] LoginDTO loginDto)
        {
            // Validar existencia del usuario en ccUsers
            bool userExists = await _appDbcontext.ccUsers.AnyAsync(u => u.User_id == loginDto.User_id);
            if (!userExists)
            {
                return BadRequest("El usuario no existe.");
            }

            // Validar fecha
            if (loginDto.fecha == default)
            {
                return BadRequest("Fecha inválida.");
            }

            var login = await _appDbcontext.ccloglogin.FindAsync(logId);
            if (login == null)
            {
                return NotFound("No se encontró el login.");
            }

            // Actualizar campos
            login.User_id = loginDto.User_id;
            login.Extension = loginDto.Extension;
            login.TipoMov = loginDto.TipoMov;
            login.fecha = loginDto.fecha;

            try
            {
                await _appDbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error al actualizar el login.");
            }

            return NoContent();
        }

        [HttpDelete]
        [Route("eliminar/{id:int}")]
        public async Task<ActionResult<LoginDTO>> EliminarLogin(int id)
        {
            var login = await _appDbcontext.ccloglogin.FindAsync(id);

            if (login == null)
            {
                return NotFound($"No se encontró un login con logId = {id}.");
            }

            _appDbcontext.ccloglogin.Remove(login);
            await _appDbcontext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [Route("generarCSV")]
        public async Task<IActionResult> GenerarCSV()
        {
            // Traer usuarios con Area y Logins (ordenados por fecha)
            var users = await _appDbcontext.ccUsers
                .Include(u => u.Area)
                .Include(u => u.Logins)
                .ToListAsync();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Login,Nombre Completo,Área,Total Horas Trabajadas");

            foreach (var user in users)
            {
                var logins = user.Logins?
                    .OrderBy(l => l.fecha)
                    .ToList() ?? new List<Login>();

                double totalHours = 0;

                // Vamos a sumar intervalos login-logout
                // Suponemos: TipoMov == 1 => Login, TipoMov == 0 => Logout
                for (int i = 0; i < logins.Count - 1; i++)
                {
                    if (logins[i].TipoMov == 1 && logins[i + 1].TipoMov == 0)
                    {
                        var diff = logins[i + 1].fecha - logins[i].fecha;
                        if (diff.TotalHours > 0)
                            totalHours += diff.TotalHours;
                        i++; // saltar el logout ya contado
                    }
                }

                var nombreCompleto = $"{user.Nombre} {user.ApellidoPaterno ?? ""} {user.ApellidoMaterno ?? ""}".Trim();
                var areaNombre = user.Area?.AreaName ?? "Sin área";

                csvBuilder.AppendLine($"{user.Login},{nombreCompleto},{areaNombre},{totalHours:F2}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(csvBytes, "text/csv", "reporte_horas_trabajadas.csv");
        }
    }
}
