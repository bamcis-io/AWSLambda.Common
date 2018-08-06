using Amazon.Lambda.Core;
using BAMCIS.AWSLambda.Common.Events.KinesisFirehose;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWSLambda.Common.Tests
{
    public class KinesisFirehoseEventLambdaFunction
    {
        public KinesisFirehoseEventLambdaFunction()
        { }

        public KinesisFirehoseTransformResponse Exec(KinesisFirehoseEvent request, ILambdaContext context)
        {
            List<KinesisFirehoseTransformedRecord> TransformedRecords = new List<KinesisFirehoseTransformedRecord>();

            foreach (KinesisFirehoseRecord Record in request.Records)
            {
                KinesisFirehoseTransformedRecord Transform = KinesisFirehoseTransformedRecord.Build(Record, x => {
                    try
                    {
                        JObject Obj = JObject.Parse(x);
                        string Row = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Join("|", Obj.ToObject<Dictionary<string, string>>().Select(y => y.Value)) + "\n"));
                        return new TransformationResult(Row, TransformationResultStatus.OK);
                    }
                    catch (Exception e)
                    {
                        return new TransformationResult(x, TransformationResultStatus.PROCESSING_FAILED);
                    }
                });
                    
                TransformedRecords.Add(Transform);
            }

            return new KinesisFirehoseTransformResponse(TransformedRecords);
        }

        public async Task<KinesisFirehoseTransformResponse> ExecAsync(KinesisFirehoseEvent request, ILambdaContext context)
        {
            List<KinesisFirehoseTransformedRecord> TransformedRecords = new List<KinesisFirehoseTransformedRecord>();

            foreach (KinesisFirehoseRecord Record in request.Records)
            {
                KinesisFirehoseTransformedRecord Transform = await KinesisFirehoseTransformedRecord.BuildAsync(Record, async x => {
                    try
                    {
                        JObject Obj = JObject.Parse(x);
                        string Row = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Join("|", Obj.ToObject<Dictionary<string, string>>().Select(y => y.Value)) + "\n"));
                        await Task.Run(() => { Thread.Sleep(1); });
                        return new TransformationResult(Row, TransformationResultStatus.OK);
                    }
                    catch (Exception e)
                    {
                        return new TransformationResult(x, TransformationResultStatus.PROCESSING_FAILED);
                    }
                });

                TransformedRecords.Add(Transform);
            }

            return new KinesisFirehoseTransformResponse(TransformedRecords);
        }
    }
}
