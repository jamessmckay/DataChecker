// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MSDF.DataChecker.Domain.Entities;
using MSDF.DataChecker.Domain.Resources;
using MSDF.DataChecker.Domain.Services.RuleExecutionLogs.Queries;
using MSDF.DataChecker.Domain.Services.Rules.Commands;
using MSDF.DataChecker.Domain.Services.Rules.Queries;
using MSDF.DataChecker.Domain.Services.RulesExecution.Commands;
using MSDF.DataChecker.WebApi.Requests;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUrlHelper _urlHelper;

        public RulesController(IMediator mediator, IUrlHelper urlHelper)
        {
            _mediator = mediator;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "An array of UserParams", typeof(IEnumerable<RuleResource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync()
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
        [SwaggerResponse(StatusCodes.Status200OK, "Rule by a given id", typeof(RuleResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAsync(Guid id)
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

        [HttpGet("Results/{id}/{databaseEnvironmentId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "An array of RuleTestResult by a given id, and database environment id", typeof(IEnumerable<RuleTestResultResource>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetResultsAsync(Guid id, Guid databaseEnvironmentId)
        {
            var results = await _mediator.Send(new GetTopResults.Query(id, databaseEnvironmentId));

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
        [SwaggerResponseHeader(StatusCodes.Status200OK, "Location", "string", "Location of the updated Rule")]
        [SwaggerResponseHeader(
            StatusCodes.Status201Created, "Location", "string", "Location of the newly created Rule")]
        public async Task<IActionResult> Post([FromBody] RuleResource command)
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
        public async Task<IActionResult> UpdateAsync([FromBody] RuleResource command)
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
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _mediator.Send(new Delete.Command(id));

            if (!result.IsSuccess)
            {
                return BadRequest(result.FailureReason);
            }

            return Accepted();
        }

        // TODO: The remainder rule execution needs to be rethought out. To many cross concerns, to many connections being spawned on a large collection.
        // As it stands now it will fail with the pool being exhausted. Secondly, these end points should be on a separate endpoint that is locked down

        // [HttpPost("Run")]
        // [ProducesResponseType(typeof(RuleTestResultResource), StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<IActionResult> RunAsync([FromBody] RuleExecutionRequest command)
        // {
        //     var result = await _mediator.Send(new ExecuteRuleByEnvironmentId.Command(command.RuleId, command.DatabaseEnvironmentId));
        //
        //     if (!result.IsSuccess)
        //     {
        //         return BadRequest(result.FailureReason);
        //     }
        //
        //     return Ok(result.Payload);
        // }

        // [HttpPost("RunDiagnosticAndReturnTable")]
        // public async Task<IActionResult> RunAndReturnTableAsync(RuleExecutionDiagnosticRequest model)
        // {
        //     var databaseEnvironment = await _databaseEnvironmentService
        //         .GetAsync(model.DatabaseEnvironmentId);
        //
        //     var result = await _executionService
        //         .ExecuteRuleDiagnosticByRuleLogIdAndEnvironmentIdAsync(model.RuleExecutionLogId, databaseEnvironment);
        //
        //     if (result != null && result.Information != null)
        //         result.Information = result.Information.Take(100).ToList();
        //
        //     return Ok(result);
        // }

        // [HttpPost("TestRun")]
        // public async Task<IActionResult> TestRunAsync([FromBody] RuleExecutionTestRequest model)
        // {
        //     var databaseEnvironment = await _databaseEnvironmentService
        //         .GetAsync(model.DatabaseEnvironmentId);
        //
        //     string connectionString = databaseEnvironment
        //         .GetConnectionString();
        //
        //     var result = await _executionService
        //         .ExecuteRuleAsync(model.RuleToTest, connectionString, databaseEnvironment.UserParams, databaseEnvironment.TimeoutInMinutes);
        //
        //     if (result != null)
        //         return Ok(result);
        //
        //     return BadRequest();
        // }

        // [HttpPost("CopyRuleTo")]
        // public async Task<IActionResult> CopyRuleToAsync([FromBody] RuleCopyToBO model)
        // {
        //     var rule = await _rulesService
        //         .CopyToAsync(model.RuleId, model.ContainerId);
        //
        //     if (rule != null)
        //         return CreatedAtAction("Get", new { id = rule.Id }, rule);
        //
        //     return BadRequest();
        // }

        // [HttpPost("SearchRules")]
        // public async Task<IActionResult> SearchRulesAsync([FromBody] SearchRulesBO model)
        // {
        //     var result = await _rulesService
        //         .SearchRulesAsync(model.Collections, model.Containers, model.Tags, model.Name, model.DiagnosticSql, model.DetailsDestination, model.Severity);
        //
        //     if (result != null)
        //         return Ok(result);
        //
        //     return NotFound();
        // }

        // [HttpPost("DeleteByIds")]
        // public async Task<IActionResult> DeleteByIdsAsync([FromBody] RulesDeleteByIdsBO model)
        // {
        //     if (model != null && model.RuleIds != null && model.RuleIds.Count > 0)
        //     {
        //         foreach (Guid ruleId in model.RuleIds)
        //         {
        //             await _rulesService.DeleteAsync(ruleId);
        //         }
        //     }
        //
        //     return Ok();
        // }

        // [HttpPost("AssignTagsByIds")]
        // public async Task<IActionResult> AssignTagsByIdsAsync([FromBody] RulesAssignTagsByIdsBO model)
        // {
        //     if (model != null && model.RuleIds != null && model.RuleIds.Count > 0
        //         && model.Tags != null && model.Tags.Count > 0)
        //     {
        //         foreach (Guid ruleId in model.RuleIds)
        //         {
        //             await _rulesService.AssignTagsToRule(ruleId, model.Tags);
        //         }
        //     }
        //
        //     return Ok();
        // }

        // [HttpPost("CopyToByIds")]
        // public async Task<IActionResult> CopyToByIdsAsync([FromBody] RulesCopyToContainerByIdsBO model)
        // {
        //     if (model != null && model.RuleIds != null && model.RuleIds.Count > 0
        //         && model.ContainerIds != null && model.ContainerIds.Count > 0)
        //     {
        //         foreach (Guid ruleId in model.RuleIds)
        //         {
        //             foreach (Guid containerId in model.ContainerIds)
        //             {
        //                 await _rulesService.CopyToAsync(ruleId, containerId);
        //             }
        //         }
        //     }
        //
        //     return Ok();
        // }

        // [HttpPost("MoveRulesToContainer")]
        // public async Task<IActionResult> MoveRuleToContainerAsync([FromBody] MoveRuleToContainerBO model)
        // {
        //     bool result = await _rulesService.MoveRuleToContainer(model.Rules, model.ContainerTo);
        //     return Ok(result);
        // }
    }
}
