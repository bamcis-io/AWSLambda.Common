using Newtonsoft.Json;
using System;

namespace BAMCIS.AWSLambda.Common.SageMaker.Serde
{
    /// <summary>
    /// Manages conversion of predicted labels into JSON by trimming decimal 0s
    /// </summary>
    public class PredictedLabelConverter : JsonConverter
    {
        #region Public Properties

        public override bool CanWrite => true;

        public override bool CanRead => false;

        #endregion

        #region Public Methods

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                try
                {
                    double Val = (double)value;

                    if (Val % 1 == 0)
                    {
                        writer.WriteValue(Int64.Parse(Val.ToString()));
                    }
                    else
                    {
                        writer.WriteValue(Val);
                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException($"The predicted label value ${value.ToString()} is not a valid double.");
                }
            }
            else
            {
                throw new ArgumentNullException("The predicted label value being written is null.");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
