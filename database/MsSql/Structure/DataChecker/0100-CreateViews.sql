CREATE VIEW destination.vw_RuleExecutionLogDetails
AS
    SELECT
        RuleExecutionLogs.Id AS RuleExecutionLogsId,
        RuleExecutionLogs.RuleId,
        RuleExecutionLogs.DatabaseEnvironmentId,
        RuleExecutionLogs.Response,
        RuleExecutionLogs.statusId,
        RuleExecutionLogs.ExecutionDate,
        RuleExecutionLogs.ExecutionTimeMs,
        RuleExecutionLogs.ExecutedSql,
        RuleExecutionLogs.RuleDetailsDestinationId,
        RuleExecutionLogs.DetailsSchema,
        Rules.Name AS RuleName,
        Rules.Description AS RuleDescription,
        Rules.ErrorMessage,
        Rules.ErrorSeverityLevel,
        Rules.Resolution,
        Rules.DiagnosticSql,
        Rules.Version,
        Rules.RuleIdentification,
        ISNULL(Rules.MaxNumberResults, DatabaseEnvironments.MaxNumberResults) AS MaxNumberResults,
        DatabaseEnvironments.Name AS DatabaseEnvironmentName,
        Destinations.Name AS DestinationTableName,
        Containers.Name AS ContainerName,
        Collections.Name AS CollectionName,
        Environments.Name AS EnvironmentsName,
        RANK() OVER (PARTITION BY Rules.id    ORDER BY RuleExecutionLogs.Id desc) AS rule_order
    FROM destination.RuleExecutionLogs
    JOIN datachecker.Rules ON
        Rules.id = RuleExecutionLogs.RuleId
    JOIN datachecker.DatabaseEnvironments ON
        DatabaseEnvironments.id = RuleExecutionLogs.DatabaseEnvironmentId
    JOIN datachecker.Containers ON
        rules.ContainerId = Containers.Id
    JOIN datachecker.Containers AS collections ON
        containers.ParentContainerId = collections.Id
    LEFT JOIN core.Catalogs AS Destinations ON
        Destinations.Id = RuleExecutionLogs.RuleDetailsDestinationId
    LEFT JOIN core.Catalogs AS Environments ON
        Environments.id = Collections.EnvironmentType;
GO
