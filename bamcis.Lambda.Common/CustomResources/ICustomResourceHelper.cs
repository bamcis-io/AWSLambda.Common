using System.Net.Http;
using System.Threading.Tasks;
using static BAMCIS.Lambda.Common.CustomResources.CustomResourceRequest;

namespace BAMCIS.Lambda.Common.CustomResources
{
    public interface ICustomResourceHelper
    {
        string FixUpResponseJson(CustomResourceResponse response, StackOperation operation);

        Task<HttpResponseMessage> UploadResponseToS3Async(CustomResourceRequest request, CustomResourceResponse response);

        Task<CustomResourceResult> PutCustomResourceResponseAsync(CustomResourceRequest request, CustomResourceResponse response);
    }
}
