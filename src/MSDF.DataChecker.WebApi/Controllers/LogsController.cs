// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MSDF.DataChecker.Domain.Resources;
using MSDF.DataChecker.Domain.Services.Logs.Commands;
using MSDF.DataChecker.Domain.Services.Logs.Queries;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogger<LogsController> _logger;
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public LogsController(ILogger<LogsController> logger, IMediator mediator, IUrlHelper urlHelper)
        {
            _logger = logger;
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "Location", "string", "Location of the newly created Log")]
        public async Task<IActionResult> Post([FromBody] LogResource command)
        {
            var result = await _mediator.Send(new Add.Command(command));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            var location = new Uri(
                $"{_urlHelper.ActionLink("Get", ControllerContext.ActionDescriptor.ControllerName)}/{result.Payload}");

            Response.GetTypedHeaders().Location = location;

            return Created(location, null);
        }

        [HttpGet]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of Tags",
            typeof(IEnumerable<LogResource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            var results = await _mediator.Send(new GetAll.Query());

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return results.Payload != null
                ? (IActionResult) Ok(results.Payload)
                : NotFound();
        }

        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "A Log for a given id", typeof(LogResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            var results = await _mediator.Send(new GetById.Query(id));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return results.Payload != null
                ? (IActionResult) Ok(results.Payload)
                : NotFound();
        }
    }
}
