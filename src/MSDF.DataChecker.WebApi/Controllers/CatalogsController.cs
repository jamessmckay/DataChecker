// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSDF.DataChecker.Domain.Resources;
using MSDF.DataChecker.Domain.Services.Catalogs.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CatalogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK,"An array of available Catalogs", typeof(IEnumerable<CatalogResource>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new GetAll.Query());

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok(result.Payload);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK,"A Catalog", typeof(CatalogResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([SwaggerParameter(Required = true)] int id)
        {
            var result = await _mediator.Send(new GetById.Query(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return result.Payload != null
                ? (IActionResult) Ok(result.Payload)
                : NotFound();
        }

        [HttpGet("GetByType")]
        [SwaggerResponse(StatusCodes.Status200OK,"A Catalog", typeof(CatalogResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([SwaggerParameter(Required = true)] string type)
        {
            var result = await _mediator.Send(new GetByType.Query(type));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return result.Payload != null
                ? (IActionResult) Ok(result.Payload)
                : NotFound();
        }
    }
}
