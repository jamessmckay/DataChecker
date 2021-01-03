// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MSDF.DataChecker.Domain.Resources;
using MSDF.DataChecker.Domain.Services.DatabaseEnvironments.Commands;
using MSDF.DataChecker.Domain.Services.DatabaseEnvironments.Queries;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class DatabaseEnvironmentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public DatabaseEnvironmentsController(IMediator mediator, IUrlHelper urlHelper, IConfiguration configuration)
        {
            _mediator = mediator;
            _urlHelper = urlHelper;
            _configuration = configuration;
        }

        [HttpGet]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of available DatabaseEnvironments",
            typeof(IEnumerable<DatabaseEnvironmentResource>))]
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
        [SwaggerResponse(StatusCodes.Status200OK, "A DatabaseEnvironment", typeof(DatabaseEnvironmentResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the updated DatabaseEnvironment")]
        [SwaggerResponseHeader(
            StatusCodes.Status201Created, "Location", "string", "Location of the newly created DatabaseEnvironment")]
        public async Task<IActionResult> Post([FromBody] DatabaseEnvironmentResource command)
        {
            var result = await _mediator.Send(new Add.Command(command));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            var location = new Uri(
                $"{_urlHelper.ActionLink("Get", ControllerContext.ActionDescriptor.ControllerName)}/{result.Payload}");

            Response.GetTypedHeaders().Location = location;

            return !result.IsUpdated
                ? (IActionResult) Created(location, null)
                : Ok();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Put([FromBody] DatabaseEnvironmentResource command)
        {
            var result = await _mediator.Send(new Update.Command(command));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new Delete.Command(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Accepted();
        }

        [HttpPost("Duplicate/{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerResponseHeader(
            StatusCodes.Status201Created, "Location", "string", "Location of the duplicated DatabaseEnvironment")]
        public async Task<IActionResult> DuplicateDatabaseEnvironment(Guid id)
        {
            var result = await _mediator.Send(new Duplicate.Command(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            var location = new Uri(
                $"{_urlHelper.ActionLink("Get", ControllerContext.ActionDescriptor.ControllerName)}/{result.Payload}");

            Response.GetTypedHeaders().Location = location;

            return Created(location, null);
        }

        [HttpGet("MapTableInformation/{id}")]
        [SwaggerResponse(
            StatusCodes.Status200OK, "A dictionary of columns by table names", typeof(IDictionary<string, List<string>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMapTable(Guid id)
        {
            var result = await _mediator.Send(new GetMapTable.Query(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return result.Payload != null
                ? (IActionResult) Ok(result.Payload)
                : NotFound();
        }

        [HttpPost("TestConnection")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TestConnection([FromBody] DatabaseEnvironmentResource resource)
        {
            var result = await _mediator.Send(new TestConnection.Command(resource));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok();
        }

        [HttpPost("TestConnection/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TestConnectionById(Guid id)
        {
            var result = await _mediator.Send(new TestConnectionById.Command(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok();
        }

        [HttpGet("GetMaxNumberResults")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult GetMaxNumberResults()
        {
            return Ok(_configuration.GetValue<int?>("MaxNumberResults") ?? 100);
        }
    }
}
