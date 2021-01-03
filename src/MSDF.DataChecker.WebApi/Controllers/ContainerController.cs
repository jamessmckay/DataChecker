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
using MSDF.DataChecker.Domain.Services.CollectionCategories.Queries;
using MSDF.DataChecker.Domain.Services.Containers.Commands;
using MSDF.DataChecker.Domain.Services.Containers.Queries;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public ContainersController(IMediator mediator, IUrlHelper urlHelper)
        {
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of available Containers", typeof(IEnumerable<CollectionCategoryResource>))]
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

        [HttpGet("ParentContainers")]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of available Parent Containers", typeof(IEnumerable<CollectionCategoryResource>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetParentContainersAsync()
        {
            var result = await _mediator.Send(new GetParentContainers.Query());

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok(result.Payload);
        }

        [HttpGet("ChildContainers")]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An array of available Child Containers", typeof(IEnumerable<CollectionCategoryResource>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetChildContainersAsync()
        {
            var result = await _mediator.Send(new GetChildContainers.Query());

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok(result.Payload);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An Available Container Category by Id", typeof(IEnumerable<CollectionCategoryResource>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var result = await _mediator.Send(new GetById.Query(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok(result.Payload);
        }

        [HttpGet("{containerId}/details/{databaseEnvironmentId}")]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An Available Container Category by Id", typeof(IEnumerable<CollectionCategoryResource>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetCollectionDetails(Guid databaseEnvironmentId, Guid containerId)
        {
            var result = await _mediator.Send(
                new GetByDatabaseEnvironmentIdAndContainerId.Query(databaseEnvironmentId, containerId));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Ok(result.Payload);
        }

        [HttpPost("AddContainerFromCommunity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseHeader(StatusCodes.Status201Created, "Location", "string", "Location of the newly created Container")]
        public async Task<ActionResult> AddContainerFromCommunity([FromBody] ContainerResource container)
        {
            //TODO Is this the correct process to add a container? How is this addressed?
            // string message = await _containerService.AddFromCommunityAsync(container);
            // return Ok(message);

            var result = await _mediator.Send(new AddContainerFromCommunity.Command(container));

            return Accepted();
        }

        [HttpGet("GetToCommunity/{id}")]
        [SwaggerResponse(
            StatusCodes.Status200OK, "An Available Community Container Category by Id",
            typeof(IEnumerable<CollectionCategoryResource>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetToCommunity(Guid id)
        {
            // TODO this method is suspect and should be reviewed again.
            var result = await _mediator.Send(new GetToCommunity.Query(id));

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
        [SwaggerResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the updated Container")]
        [SwaggerResponseHeader(
            StatusCodes.Status201Created, "Location", "string", "Location of the newly created DatabaseEnvironment")]
        public async Task<IActionResult> Post([FromBody] ContainerResource container)
        {
            var result = await _mediator.Send(new Add.Command(container));

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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _mediator.Send(new Delete.Command(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Accepted();
        }

        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAsync([FromBody] ContainerResource command)
        {
            var result = await _mediator.Send(new Update.Command(command));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return NoContent();
        }

        [HttpPut("SetDefault/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetDefaultAsync(Guid command)
        {
            var result = await _mediator.Send(new SetDefault.Command(command));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return NoContent();
        }

        [HttpGet("GetByName")]
        [SwaggerResponse(StatusCodes.Status200OK,"A Container", typeof(ContainerResource))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByNameAsync([SwaggerParameter(Required = true)] string name)
        {
            var result = await _mediator.Send(new GetByName.Query(name));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return result.Payload != null
                ? (IActionResult) Ok(result.Payload)
                : NotFound();
        }

        // TODO: this needs re-evaluation for validity and if it is needed
        // [HttpPost("ValidateDestinationTable")]
        // public async Task<ActionResult> ValidateDestinationTableAsync([FromBody] ContainerDestinationBO model)
        // {
        //     if (model == null) return Ok(true);
        //     bool result = await _containerService.ValidateDestinationTableAsync(model);
        //     return Ok(result);
        // }
    }
}
