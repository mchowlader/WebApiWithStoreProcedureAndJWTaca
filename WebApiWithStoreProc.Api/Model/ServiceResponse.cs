using System.Collections.Generic;

namespace WebApiWithStoreProc.Api.Model
{
    public class ServiceResponse<TEntity> where TEntity : class
    {
        public TEntity Data { get; set; }
        public List<string> Message { get; set; }
        public bool Success { get; set; }

        public static ServiceResponse<TEntity> UploadSuccessfully(TEntity data, string message = null)
        {
            return new ServiceResponse<TEntity>
            {
                Data = data,
                Message = new List<string> { message ?? "Data upload successfully." },
                Success = true
            };
        }
    }
}
