using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIInstructor.src.AIKisiOzellik.DTO;
using AIInstructor.src.AIKisiOzellik.Service;

namespace AIInstructor.src.AIKisiOzellik.Controller
{
    [ApiController]
    [Route("api/senaryo/{senaryoId:guid}/ozellik")]
    public class AIKisiOzellikController : ControllerBase
    {
        private readonly IAIKisiOzellikService service;

        public AIKisiOzellikController(IAIKisiOzellikService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize(Roles = "DersYetkilisi,Ogrenci")]
        public async Task<ActionResult<IEnumerable<AIKisiOzellikDto>>> Get(Guid senaryoId)
        {
            var result = await service.GetBySenaryoIdAsync(senaryoId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "DersYetkilisi")]
        public async Task<ActionResult<AIKisiOzellikDto>> Create(Guid senaryoId, [FromBody] AIKisiOzellikCreateDto dto)
        {
            dto.SenaryoId = senaryoId;
            var created = await service.CreateAsync(dto);
            return Ok(created);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "DersYetkilisi")]
        public async Task<IActionResult> Delete(Guid senaryoId, Guid id)
        {
            await service.DeleteAsync(id);
            return NoContent();
        }
    }
}
