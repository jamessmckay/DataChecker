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
using MSDF.DataChecker.Domain.Services.UserParams.Commands;
using MSDF.DataChecker.Domain.Services.UserParams.Queries;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserParamController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public UserParamController(IMediator mediator, IUrlHelper urlHelper)
        {
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of UserParams",
            typeof(IEnumerable<UserParamResource>))]
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

        [HttpGet("GetByDatabaseEnvironmentId/{id}")]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of UserParams for a given DatabaseEnvironmentId",
            typeof(IEnumerable<UserParamResource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByExample(Guid id)
        {
            var results = await _mediator.Send(new GetByDatabaseEnvironmentId.Query(id));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return results.Payload != null
                ? (IActionResult) Ok(results.Payload)
                : NotFound();
        }


        [HttpGet("{id}")]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An UserParam for a given id",
            typeof(UserParamResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(Guid id)
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
        [SwaggerResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the updated UserParam")]
        [SwaggerResponseHeader(
            StatusCodes.Status201Created, "Location", "string", "Location of the newly created UserParam")]
        public async Task<IActionResult> Post([FromBody] UserParamResource command)
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

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Put([FromBody] UserParamResource command)
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

        [HttpPost("UpdateUserParams/{databaseEnvironmentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateUserParams([FromBody] List<UserParamResource> models, Guid databaseEnvironmentId)
        {
            //TODO: Should we be returning a 200 with a list of updates resources? Should this be a 204 NoContent?
            var result = await _mediator.Send(new UpdateUserParams.Command(databaseEnvironmentId, models));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok(result.Payload);
        }
    }
}
