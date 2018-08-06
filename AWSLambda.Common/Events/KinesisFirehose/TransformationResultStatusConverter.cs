using Newtonsoft.Json;
using System;

namespace BAMCIS.AWSLambda.Common.Events.KinesisFirehose
{
    /// <summary>
    /// Converts the transformation result status enum to string values
    /// </summary>
    public class TransformationResultStatusConverter : JsonConverter
    {
        #region Public Properties

        public override bool CanRead => true;

        public override bool CanWrite => true;

        #endregion

        #region Public Methods

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TransformationResultStatus);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string Value = reader.Value.ToString().ToLower();

            switch (Value)
            {
                case "ok":
                    {
                        return TransformationResultStatus.OK;
                    }
                case "processingfailed":
                    {
                        return TransformationResultStatus.PROCESSING_FAILED;
                    }
                case "dropped":
                    {
                        return TransformationResultStatus.DROPPED;
                    }
                default:
                    {
                        throw new ArgumentException($"Unexpected transformation result status value of {Value}");
                    }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TransformationResultStatus Result = (TransformationResultStatus)value;

            switch (Result)
            {
                case TransformationResultStatus.OK:
                    {
                        writer.WriteValue("Ok");
                        break;
                    }
                case TransformationResultStatus.DROPPED:
                    {
                        writer.WriteValue("Dropped");
                        break;
                    }
                case TransformationResultStatus.PROCESSING_FAILED:
                    {
                        writer.WriteValue("ProcessingFailed");
                        break;
                    }
                default:
                    {
                        throw new ArgumentException($"Unknown transformation result status value: {Result.ToString()}.");
                    }
            }
        }

        #endregion
    }
}
