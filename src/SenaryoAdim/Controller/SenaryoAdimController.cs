using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIInstructor.src.SenaryoAdim.DTO;
using AIInstructor.src.SenaryoAdim.Service;

namespace AIInstructor.src.SenaryoAdim.Controller
{
    [ApiController]
    [Route("api/senaryo/{senaryoId:guid}/adim")]
    public class SenaryoAdimController : ControllerBase
    {
        private readonly ISenaryoAdimService service;

        public SenaryoAdimController(ISenaryoAdimService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize(Roles = "DersYetkilisi,Ogrenci")]
        public async Task<ActionResult<IReadOnlyList<SenaryoAdimDto>>> Get(Guid senaryoId)
        {
            var result = await service.GetBySenaryoIdAsync(senaryoId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "DersYetkilisi")]
        public async Task<ActionResult<SenaryoAdimDto>> Create(Guid senaryoId, [FromBody] SenaryoAdimCreateDto dto)
        {
            dto.SenaryoId = senaryoId;
            var created = await service.CreateOrUpdateAsync(dto);
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
