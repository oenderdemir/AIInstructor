using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIInstructor.src.Senaryolar.DTO;
using AIInstructor.src.Senaryolar.Service;

namespace AIInstructor.src.Senaryolar.Controller
{
    [ApiController]
    [Route("api/senaryo")]
    public class SenaryoController : ControllerBase
    {
        private readonly ISenaryoService senaryoService;

        public SenaryoController(ISenaryoService senaryoService)
        {
            this.senaryoService = senaryoService;
        }

        [HttpGet]
        [Authorize(Roles = "DersYetkilisi,Ogrenci")]
        public async Task<ActionResult<IEnumerable<SenaryoDto>>> GetAll()
        {
            var result = await senaryoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "DersYetkilisi,Ogrenci")]
        public async Task<ActionResult<SenaryoDto>> GetById(Guid id)
        {
            var result = await senaryoService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "DersYetkilisi")]
        public async Task<ActionResult<SenaryoDto>> Create([FromBody] CreateSenaryoDto dto)
        {
            var created = await senaryoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "DersYetkilisi")]
        public async Task<ActionResult<SenaryoDto>> Update(Guid id, [FromBody] CreateSenaryoDto dto)
        {
            var updated = await senaryoService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "DersYetkilisi")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await senaryoService.DeleteAsync(id);
            return NoContent();
        }
    }
}
