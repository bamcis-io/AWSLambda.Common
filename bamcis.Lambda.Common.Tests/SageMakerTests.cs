using BAMCIS.Lambda.Common.SageMaker;
using Newtonsoft.Json;
using Xunit;

namespace Lambda.Common.Tests
{
    public class SageMakerTests
    {
        [Fact]
        public void LinearLearnerBinaryResponseTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""score"":0.4,
""predicted_label"":0
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            LinearLearnerBinaryInferenceResponse Response = JsonConvert.DeserializeObject<LinearLearnerBinaryInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        
        }

        [Fact]
        public void LinearLearnerMulticlassResponseTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""score"":[0.1,0.2,0.4,0.3],
""predicted_label"":2
},
{
""score"":[0.2,0.3,0.5,0.7],
""predicted_label"":4
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            LinearLearnerMulticlassInferenceResponse Response = JsonConvert.DeserializeObject<LinearLearnerMulticlassInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);

        }

        [Fact]
        public void LinearLearnerRegressionResponseTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""score"":0.4
},
{
""score"":0.98
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            LinearLearnerRegressionInferenceResponse Response = JsonConvert.DeserializeObject<LinearLearnerRegressionInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }

        [Fact]
        public void FactorizationBinaryTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""score"":0.4,
""predicted_label"":0
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            FactorizationMachineBinaryInferenceResponse Response = JsonConvert.DeserializeObject<FactorizationMachineBinaryInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }

        [Fact]
        public void FactorizationRegressionTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""score"":0.4
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            FactorizationMachineRegressionInferenceResponse Response = JsonConvert.DeserializeObject<FactorizationMachineRegressionInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }

        [Fact]
        public void DeepARTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""quantiles"":{
""0.9"":[
0.1,0.2,0.3
],
""0.5"":[
1.1,0.4,0.66
]
},
""samples"":[
0.4,0.1
],
""mean"":[
0.9,0.8
]
},
{
""quantiles"":{
""0.9"":[
0.1,0.2,0.3
],
""0.5"":[
1.1,0.4,0.66
]
},
""samples"":[
0.4,0.1
],
""mean"":[
0.9,0.8
]
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            DeepARForecastInferenceResponse Response = JsonConvert.DeserializeObject<DeepARForecastInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }

        [Fact]
        public void KMeansTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""closest_cluster"":1.0,
""distance_to_cluster"":3.0
},
{
""closest_cluster"":2.0,
""distance_to_cluster"":5.0
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            KMeansInferenceResponse Response = JsonConvert.DeserializeObject<KMeansInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }

        [Fact]
        public void KNearestNeighborTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""predicted_label"":1.0
},
{
""predicted_label"":2.0
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            KNearestNeighborInferenceResponse Response = JsonConvert.DeserializeObject<KNearestNeighborInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }

        [Fact]
        public void NeuralTopicModelTest()
        {
            // ARRANGE

            string Json = @"{
""predictions"":[
{
""topic_weights"":[
0.02,0.1,0.0,0.25
]
},
{
""topic_weights"":[
0.25,0.067,0.0,0.1
]
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            NeuralTopicModelInferenceResponse Response = JsonConvert.DeserializeObject<NeuralTopicModelInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }

        [Fact]
        public void PCATest()
        {
            // ARRANGE

            string Json = @"{
""projections"":[
{
""projection"":[
1.0,2.0,3.0,4.0
]
},
{
""projection"":[
1.1,2.0,3.1,4.0
]
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            PrincipalComponentAnalysisInferenceResponse Response = JsonConvert.DeserializeObject<PrincipalComponentAnalysisInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }

        [Fact]
        public void RCFTest()
        {
            // ARRANGE

            string Json = @"{
""scores"":[
{
""score"":0.02
},
{
""score"":0.25
}
]
}";
            Json = Json.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");

            // ACT
            RandomCutForestInferenceResponse Response = JsonConvert.DeserializeObject<RandomCutForestInferenceResponse>(Json);
            string Json2 = JsonConvert.SerializeObject(Response, Formatting.None);

            // ASSERT
            Assert.Equal(Json, Json2);
        }
    }
}
