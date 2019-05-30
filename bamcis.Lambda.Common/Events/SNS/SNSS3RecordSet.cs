using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BAMCIS.Lambda.Common.Events.SNS
{
    /// <summary>
    /// Represents the top level JSON object in the SNS message body, 
    /// the array of "records"
    /// </summary>
    public class SNSS3RecordSet
    {
        #region Public Properties

        /// <summary>
        /// The records contained inside the SNS message body text
        /// </summary>
        public IEnumerable<SNSS3Record> Records { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor for creating the record set
        /// </summary>
        /// <param name="records"></param>
        [JsonConstructor]
        public SNSS3RecordSet(IEnumerable<SNSS3Record> records)
        {
            this.Records = records ?? throw new ArgumentNullException("records");
        }

        #endregion
    }
}
