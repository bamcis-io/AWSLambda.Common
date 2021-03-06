﻿using Newtonsoft.Json;

namespace BAMCIS.Lambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// The possible transformation result status options
    /// for an individual record
    /// </summary>
    [JsonConverter(typeof(TransformationResultStatusConverter))]
    public enum TransformationResultStatus
    {
        OK,
        DROPPED,
        PROCESSING_FAILED
    }
}
