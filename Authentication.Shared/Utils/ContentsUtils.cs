
using System.Text;
using System.Text.Json;
using Authentication.Shared.Common;

namespace Authentication.Shared.Utils;

public class ContentsUtils
{
    public static HttpContent GetHttpContent<T>(T request, JsonSerializerOptions jsonSerializerOptions,
        ContentType contentType = ContentType.Json)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        switch (contentType)
        {
            case ContentType.Json:
                var jsonRequest = JsonSerializer.Serialize(request, jsonSerializerOptions);
                return new StringContent(jsonRequest, Encoding.UTF8, CommonConsts.AcceptValue);
            
            case ContentType.FormData:
                var formDataContent = new MultipartFormDataContent();
                
                foreach (var property in typeof(T).GetProperties())
                {
                    formDataContent.Add(new StringContent(property.GetValue(request)?.ToString() ?? ""), property.Name);
                }
                return formDataContent;
            
            case ContentType.FormUrlEncoded:
                var urlEncodedContent = new FormUrlEncodedContent(
                    from prop in typeof(T).GetProperties()
                    select new KeyValuePair<string, string>(prop.Name, prop.GetValue(request)?.ToString() ?? "")
                );

                return urlEncodedContent;
            default:
                throw new ArgumentException("Invalid content type.", nameof(contentType));
        }
    }
}