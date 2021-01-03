// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MSDF.DataChecker.Domain.Providers
{
    public interface IDatabaseMapProvider
    {
        Task<string> GetJson(string connectionString);

        Task<IDictionary<string, List<string>>> Get(string connectionString);
    }

    public class DatabaseMapProvider : IDatabaseMapProvider
    {
        private readonly ILogger<DatabaseMapProvider> _logger;

        public DatabaseMapProvider(ILogger<DatabaseMapProvider> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetJson(string connectionString)
        {
            return JsonSerializer.Serialize(await Get(connectionString));
        }

        public async Task<IDictionary<string, List<string>>> Get(string connectionString)
        {
            var columnsByTable = new Dictionary<string, List<string>>();

            try
            {
                await using var conn = new SqlConnection(connectionString);

                await conn.OpenAsync();
                var schema = conn.GetSchema("Tables");
                var sourceTableRows = conn.GetSchema("Columns");

                var list = new List<string>();

                foreach (DataRow row in schema.Rows)
                {
                    var columns = row.Table.Rows;
                    var table = row[2].ToString();

                    if (!list.Contains(table))
                    {
                        var rowList = new List<string>();

                        foreach (DataRow column in sourceTableRows.Rows)
                        {
                            if (table == column[2].ToString())
                            {
                                if (!list.Contains(column[3].ToString()))
                                {
                                    rowList.Add(column[3].ToString());
                                }
                            }
                        }

                        columnsByTable.Add(table, rowList);
                        list.Add(table);
                    }
                }
            }
            catch (Exception e)
            {
                // TODO: this should bubble up
                _logger.LogDebug(e.ToString());
            }

            return columnsByTable;
        }
    }
}
