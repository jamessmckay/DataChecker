// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using MSDF.DataChecker.Domain.Resources;

namespace MSDF.DataChecker.Domain.Entities
{
    public class RuleTestResult
    {
        public RuleTestResult()
        {
            TestResults = new List<RuleExecutionLog>();
        }

        public int Id { get; set; }

        public Rule Rule { get; set; }

        public int Result { get; set; }

        public long ExecutionTimeMs { get; set; }

        public bool Evaluation { get; set; }

        public Status Status { get; set; }

        public DateTime? LastExecuted { get; set; }

        public List<RuleExecutionLog> TestResults { get; set; }

        public string ErrorMessage { get; set; }

        public string ExecutedSql { get; set; }

        public string DiagnosticSql { get; set; }

        public int? RuleDetailsDestinationId { get; set; }
    }
}
