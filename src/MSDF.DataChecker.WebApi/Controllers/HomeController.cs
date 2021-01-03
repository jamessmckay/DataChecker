// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace MSDF.DataChecker.WebApi.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/")]
    [Produces(MediaTypeNames.Application.Json)]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var apiDocsUrl = new UriBuilder(Request.GetEncodedUrl());
            apiDocsUrl.Path += "api-docs";
            apiDocsUrl.Query = null;

            var hangfireDashboardUrl = new UriBuilder(Request.GetEncodedUrl());
            hangfireDashboardUrl.Path += "dashboards/hangfire";
            hangfireDashboardUrl.Query = null;

            return Ok(
                new
                {
                    Version = "1.0.0",
                    Links = new {ApiDocs = apiDocsUrl.Uri.ToString(), HangfireDashboard = hangfireDashboardUrl.Uri.ToString() },
                    OpenApiMetadata = new {V1 = apiDocsUrl.Uri + "/v1/openapi.json"}
                });
        }
    }
}
