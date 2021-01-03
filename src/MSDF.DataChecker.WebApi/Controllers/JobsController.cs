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
using MSDF.DataChecker.Domain.Resources;
using MSDF.DataChecker.Domain.Services.Jobs.Commands;
using MSDF.DataChecker.Domain.Services.Jobs.Queries;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public JobsController(IMediator mediator, IUrlHelper urlHelper)
        {
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of Jobs",
            typeof(IEnumerable<JobResource>))]
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
        [SwaggerResponse(StatusCodes.Status200OK, "A Log for a given id", typeof(JobResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string id)
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

        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the updated Job")]
        [SwaggerResponseHeader(
            StatusCodes.Status201Created, "Location", "string", "Location of the newly created Job")]
        public async Task<IActionResult> Post([FromBody] JobResource command)
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

        [HttpPut("Update/")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromBody] JobResource command)
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
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _mediator.Send(new Delete.Command(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Accepted();
        }

        [HttpPost("Enqueue/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Enqueue(long id)
        {
            var results = await _mediator.Send(new Enqueue.Command(id));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return Accepted();
        }

        [HttpPost("RunAndForget")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RunAndForget(JobResource command)
        {
            var results = await _mediator.Send(new RunAndForget.Command(command));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return Accepted();
        }
    }
}
