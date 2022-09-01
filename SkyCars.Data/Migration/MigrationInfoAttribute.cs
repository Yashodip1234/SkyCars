using System;
using System.Globalization;
using FluentMigrator;

namespace SkyCars.Data.Migrations
{
    /// <summary>
    /// Attribute for a migration
    /// </summary>
    public partial class MigrationInfoAttribute : MigrationAttribute
    {
        private static readonly string[] _dateFormats = { "yyyy-MM-dd HH:mm:ss", "yyyy.MM.dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss:fffffff", "yyyy.MM.dd HH:mm:ss:fffffff", "yyyy/MM/dd HH:mm:ss:fffffff" };

        /// <summary>
        /// Initializes a new instance of the MigrationInfoAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        public MigrationInfoAttribute(string dateTime) :
            base(DateTime.ParseExact(dateTime, _dateFormats, CultureInfo.InvariantCulture).Ticks, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MigrationInfoAttribute class
        /// </summary>
        /// <param name="dateTime">The migration date time string to convert on version</param>
        /// <param name="description">The migration description</param>
        public MigrationInfoAttribute(string dateTime, string description) :
            base(DateTime.ParseExact(dateTime, _dateFormats, CultureInfo.InvariantCulture).Ticks, description)
        {
        }
    }
}
