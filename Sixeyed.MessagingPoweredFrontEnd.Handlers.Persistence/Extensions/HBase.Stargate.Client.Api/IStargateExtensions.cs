using HBase.Stargate.Client.Models;
using System.Collections.Generic;
using System.Linq;

namespace HBase.Stargate.Client.Api
{
    public static class IStargateExtensions
    {
        public static void BootstrapSchema(this IStargate client)
        {
            var tables = client.GetTableNames();

            if (!tables.Contains("mail"))
            {
                client.CreateTable(new TableSchema
                {
                    Name = "mail",
                    Columns = new List<ColumnSchema>
                    {
                        new ColumnSchema
                        {
                            Name = "m"
                        }
                    }
                });
            }
        }
    }
}