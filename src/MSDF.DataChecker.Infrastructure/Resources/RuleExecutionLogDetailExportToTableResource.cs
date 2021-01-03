// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MSDF.DataChecker.Domain.Resources
{
    public class RuleExecutionLogDetailExportToTableResource
    {
        public string TableName { get; set; }

        public bool AlreadyExist { get; set; }

        public bool Created { get; set; }

        public string Message { get; set; }

        public int RuleExecutionLogId { get; set; }
    }
}