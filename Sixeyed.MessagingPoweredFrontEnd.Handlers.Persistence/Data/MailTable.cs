using HBase.Stargate.Client.Api;
using HBase.Stargate.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixeyed.MessagingPoweredFrontEnd.Handlers.Persistence.Data
{
    /// <summary>
    /// Mail messages
    ///  create 'mail', 'm'
    /// Row key = {userId}|{mailPeriod}
    /// m = messages
    ///     m:{timestamp} = {content} 
    /// </summary>
    public class MailTable
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IStargate _client;

        public MailTable(IStargate client)
        {
            _client = client;
        }

        public void PutNew(string from, string content, DateTime date)
        {
            var rowKey = string.Format("{0}|{1}", from, date.ToString("yyyyMMddHH"));
            var cellSet = new CellSet
            {
                Table = "mail"
            };

            var timestamp = ToUnixMilliseconds(date);
            var identifier = GetIdentifier(rowKey, "m", timestamp.ToString());
            cellSet.Add(new Cell(identifier, content));

            _client.WriteCells(cellSet);
        }

        protected Identifier GetIdentifier(string rowKey, string column, string qualifier)
        {
            return new Identifier
            {
                Table = "mail",
                Row = rowKey,
                CellDescriptor = new HBaseCellDescriptor
                {
                    Column = column,
                    Qualifier = qualifier
                }
            };
        }

        private static long ToUnixMilliseconds(DateTime dateTime)
        {
            return (long)(dateTime.Subtract(UnixEpoch)).TotalMilliseconds;
        }
    }
}
