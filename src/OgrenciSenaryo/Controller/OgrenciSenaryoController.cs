using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIInstructor.src.OgrenciSenaryo.DTO;
using AIInstructor.src.OgrenciSenaryo.Service;

namespace AIInstructor.src.OgrenciSenaryo.Controller
{
    [ApiController]
    [Route("api/ogrenci-senaryo")]
    public class OgrenciSenaryoController : ControllerBase
    {
        private readonly IOgrenciSenaryoService service;

        public OgrenciSenaryoController(IOgrenciSenaryoService service)
        {
            this.service = service;
        }

        [HttpGet("ogrenci/{ogrenciId:guid}")]
        [Authorize(Roles = "DersYetkilisi,Ogrenci")]
        public async Task<ActionResult<IEnumerable<OgrenciSenaryoDto>>> GetByOgrenci(Guid ogrenciId)
        {
            var result = await service.GetByOgrenciIdAsync(ogrenciId);
            return Ok(result);
        }

        [HttpGet]
        [Route("~/api/ogrenci/{ogrenciId:guid}/senaryolar")]
        [Authorize(Roles = "DersYetkilisi,Ogrenci")]
        public async Task<ActionResult<IEnumerable<OgrenciSenaryoDto>>> GetSenaryolarForStudent(Guid ogrenciId)
        {
            var result = await service.GetByOgrenciIdAsync(ogrenciId);
            return Ok(result);
        }

        [HttpPost("assign")]
        [Authorize(Roles = "DersYetkilisi")]
        public async Task<ActionResult<OgrenciSenaryoDto>> Assign([FromBody] OgrenciSenaryoAssignDto dto)
        {
            var created = await service.AssignAsync(dto);
            return Ok(created);
        }

        [HttpPost("complete")]
        [Authorize(Roles = "DersYetkilisi,Ogrenci")]
        public async Task<ActionResult<OgrenciSenaryoDto>> Complete([FromBody] OgrenciSenaryoCompleteDto dto)
        {
            var updated = await service.CompleteAsync(dto);
            return Ok(updated);
        }
    }
}
