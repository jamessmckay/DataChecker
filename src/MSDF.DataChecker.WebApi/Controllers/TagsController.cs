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
using MSDF.DataChecker.Domain.Services.Tags.Commands;
using MSDF.DataChecker.Domain.Services.Tags.Queries;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public TagsController(IMediator mediator, IUrlHelper urlHelper)
        {
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the updated Tag")]
        [SwaggerResponseHeader(
            StatusCodes.Status201Created, "Location", "string", "Location of the newly created Tag")]
        public async Task<IActionResult> Post([FromBody] TagResource command)
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
        public async Task<ActionResult> Put([FromBody] TagResource command)
        {
            var result = await _mediator.Send(new Update.Command(command));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return NoContent();
        }

        [HttpPost("SearchByTags")]
        [SwaggerResponse(StatusCodes.Status200OK, "An array of Tag for a given rule id", typeof(IEnumerable<TagResource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchByTagsAsync([FromBody] List<TagResource> command)
        {
            var results = await _mediator.Send(new SearchByTags.Command(command));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return results.Payload != null
                ? (IActionResult) Ok(results.Payload)
                : NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var result = await _mediator.Send(new Delete.Command(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Accepted();
        }

        [HttpGet]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of Tags",
            typeof(IEnumerable<TagResource>))]
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
        [SwaggerResponse(StatusCodes.Status200OK, "A Tag for a given id", typeof(TagResource))]
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

        [HttpGet("GetByContainer/{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "An array of Tag for a given container id", typeof(IEnumerable<TagResource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByContainerAsync(Guid id)
        {
            var results = await _mediator.Send(new GetByContainerId.Query(id));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return results.Payload != null
                ? (IActionResult) Ok(results.Payload)
                : NotFound();
        }

        [HttpGet("GetByRule/{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "An array of Tag for a given rule id", typeof(IEnumerable<TagResource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRuleAsync(Guid id)
        {
            var results = await _mediator.Send(new GetByRuleId.Query(id));

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
