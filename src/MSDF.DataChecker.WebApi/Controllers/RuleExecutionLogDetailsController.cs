// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSDF.DataChecker.Domain.Services.RuleExecutionLogDetails.Commands;
using MSDF.DataChecker.Domain.Services.RuleExecutionLogDetails.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RuleExecutionLogDetailsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RuleExecutionLogDetailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("LastRuleExecutionLogByEnvironmentAndRule/{environmentId}/{ruleId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "An identity to an execution log detail", typeof(int))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LastRuleExecutionLogByEnvironmentAndRule(Guid environmentId, Guid ruleId)
        {
            var results = await _mediator.Send(new GetLastRuleExecutionLogIdByEnvironmentAndRule.Query(ruleId, environmentId));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return results.Payload != null
                ? (IActionResult) Ok(results.Payload)
                : NotFound();
        }

        [HttpGet("RuleExecutionLogAsync/{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "An execution log detail for a given id", typeof(int))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRuleExecutionLogAsync(int id)
        {
            var results = await _mediator.Send(new GetByRuleExecutionLogId.Query(id));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            if (results.Payload == null)
            {
                return NotFound();
            }

            if (results.Payload.Columns.Any())
            {
                return Ok(results.Payload);
            }

            return Ok();
        }

        // TODO: this needs to move to middleware
        [HttpGet("ExportToCsvAsync/{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "An exported execution log detail for a given id", typeof(int))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("text/csv")]
        public async Task<IActionResult> ExportToCsvAsync(int id)
        {
            var result = await _mediator.Send(new GetByRuleExecutionLogId.Query(id));

            if (result.Payload == null || !result.Payload.Columns.Any())
            {
                return NotFound();
            }

            var sb = new StringBuilder();
            var model = result.Payload;

            string headers = string.Empty;

            for (int i = 0; i < model.Columns.Count; i++)
            {
                string column = model.Columns[i];
                headers += $"{column}";

                if (i + 1 < model.Columns.Count)
                {
                    headers += ",";
                }
            }

            sb.AppendLine(headers);

            for (int i = 0; i < model.Rows.Count; i++)
            {
                var row = model.Rows[i];
                string rowsValues = string.Empty;

                for (int j = 0; j < model.Columns.Count; j++)
                {
                    string column = model.Columns[j];
                    string value = row.GetValueOrDefault(column);

                    rowsValues += $"\"{value}\"";

                    if (j + 1 < model.Columns.Count)
                    {
                        rowsValues += ",";
                    }
                }

                sb.AppendLine(rowsValues);
            }

            return File(
                Encoding.UTF8.GetBytes(sb.ToString()), "text/csv",
                $"RuleExecutionLogDetails-{model.RuleExecutionLogId}.csv");
        }

        // TODO: Should this be a post?
        [HttpGet("ExportToTableAsync/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportToTableAsync(int id)
        {
            var results = await _mediator.Send(new ExportToTableByRuleExecutionLogId.Command(id));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return results.Payload != null
                ? (IActionResult) Ok(results.Payload)
                : Ok();
        }

        [HttpGet("ExecuteDiagnosticSqlFromLog/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExecuteDiagnosticSqlFromLog(int id)
        {
            var results = await _mediator.Send(new GetExecutionDiagnosticSqlByLogId.Query(id));

            if (!results.IsSuccess)
            {
                return BadRequest(results.FailureReason);
            }

            return results.Payload != null
                ? (IActionResult) Ok(results.Payload)
                : Ok();
        }
    }
}
