using System.Runtime.Serialization;
using System.ServiceModel;
using File;
using Grpc.Net.Client;

namespace AdvertisingSystem.Application.Grpc;

[DataContract]
public class FileRequest
{
    [DataMember(Order = 1)]
    public byte[] FileContent { get; set; }
    
    [DataMember(Order = 2)]
    public string FileName { get; set; }
    
    [DataMember(Order = 3)]
    public long AdvertisementId { get; set; }
}

[DataContract]
public class FileResponse
{
    [DataMember(Order = 1)]
    public bool Success { get; set; }
    
    [DataMember(Order = 2)]
    public string Message { get; set; }
}

[ServiceContract]
public interface IFileService
{
    [OperationContract]
    Task<FileResponse> UploadFile(FileRequest request);

    
}
