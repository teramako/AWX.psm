using AWX.Resources;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AWX
{
    public enum AcceptType
    {
        Json, Text, Html
    }
    public enum Method
    {
        GET, POST, PUT, PATCH, DELETE, OPTIONS
    }
    /// <summary>
    /// Rest API client for AWX
    /// </summary>
    public class RestAPI
    {
        /// <summary>
        /// Single HttpClient for AWX
        /// </summary>
        static public HttpClient Client
        {
            get
            {
                if (_client != null) return _client;
                var config = ApiConfig.Instance;
                _client = new HttpClient()
                {
                    BaseAddress = config.Origin,
                    DefaultRequestVersion = HttpVersion.Version11,
                };
                _client.DefaultRequestHeaders.Add("Accept", "application/json");
                if (!string.IsNullOrEmpty(config.Token))
                {
                    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.Token}");
                }
                return _client;
            }
            private set
            {
                _client?.Dispose();
                _client = value;
            }
        }
        static private HttpClient? _client = null;
        public static void SetClient(ApiConfig config)
        {
            var uri = config.Origin;
            var client = Client;
            if (client.BaseAddress == null)
            {
                client.BaseAddress = uri;
            }
            else if (client.BaseAddress.Scheme != uri.Scheme || client.BaseAddress.Authority != uri.Authority)
            {
                client = new HttpClient()
                {
                    BaseAddress = uri,
                    DefaultRequestVersion = HttpVersion.Version11,
                };
                Client = client;
            }
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            if (!string.IsNullOrEmpty(config.Token))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.Token}");
            }
        }
        private static async Task<RestAPIException> CreateException(HttpResponseMessage response, string contentType)
        {
            var msg1 = $"{response.StatusCode:d} ({response.ReasonPhrase}): ";
            var msg2 = (response.RequestMessage != null && response.RequestMessage.RequestUri != null)
                       ? $" on {response.RequestMessage.Method} {response.RequestMessage.RequestUri.PathAndQuery}"
                       : string.Empty;
            switch (contentType)
            {
                case JsonContentType:
                    var error = await response.Content.ReadAsStringAsync();
                    return new RestAPIException($"{msg1}{error}{msg2}", response);
                default:
                    return new RestAPIException($"{msg1}{contentType}{msg2}", response);
            }

        }
        /// <summary>
        /// Handle HTTP response contents.<br/>
        /// Returns JSON deserialized object if the HTTP status is success code and <c>Content-Type</c> is <c>application/json</c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns>The contents object or exception is wrapped <see cref="RestAPIResult{T}"/></returns>
        /// <exception cref="RestAPIException"></exception>"
        private static async Task<RestAPIResult<T>> HandleResponse<T>(HttpResponseMessage response) where T : class
        {
            var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(string))
                {
                    long contentLength = response.Content.Headers.ContentLength ?? 0;
                    T? stringContents = (contentLength == 0 ? string.Empty : await response.Content.ReadAsStringAsync()) as T;
                    if (stringContents == null)
                        throw new NullReferenceException();
                    return new RestAPIResult<T>(response, stringContents);
                }
                if (contentType == JsonContentType)
                {
                    try
                    {
                        var obj = await response.Content.ReadFromJsonAsync<T>(Json.DeserializeOptions)
                            ?? throw new RestAPIException("Failed to read JSON. The result is null.", response);
                        return new RestAPIResult<T>(response, obj);

                    }
                    catch (Exception ex)
                    {
                        throw new RestAPIException($"Could not deserialize JSON to {typeof(T)}", response, ex);
                    }
                }
                else
                {
                    throw new RestAPIException($"Invalid Content-Type: {contentType}", response);

                }
            }
            // Error handling
            throw await CreateException(response, contentType);
        }
        /// <summary>
        /// Handle HTTP response contents.<br/>
        /// Returns JSON deserialized object if the HTTP status is success code and <c>Content-Type</c> is <c>application/json</c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns>The contents object or exception is wrapped <see cref="RestAPIResult{T}"/></returns>
        /// <exception cref="RestAPIException"></exception>"
        private static async Task<RestAPIPostResult<T>> HandlePostResponse<T>(HttpResponseMessage response) where T : class
        {
            var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            if (response.IsSuccessStatusCode)
            {
                long contentLength = response.Content.Headers.ContentLength ?? 0;
                if (typeof(T) == typeof(string))
                {
                    string stringContents = (contentLength == 0 ? string.Empty : await response.Content.ReadAsStringAsync());
                    return new RestAPIPostResult<T>(response, stringContents as T);
                }
                else if (contentLength == 0 || response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new RestAPIPostResult<T>(response);
                }
                else if (contentType == JsonContentType)
                {
                    try
                    {
                        var obj = await response.Content.ReadFromJsonAsync<T>(Json.DeserializeOptions)
                            ?? throw new RestAPIException("Failed to read JSON. The result is null.", response);
                        return new RestAPIPostResult<T>(response, obj);

                    }
                    catch (Exception ex)
                    {
                        throw new RestAPIException($"Could not deserialize JSON to {typeof(T)}", response, ex);
                    }
                }
                else
                {
                    throw new RestAPIException($"Invalid Content-Type: {contentType}", response);
                }
            }
            // Error handling
            throw await CreateException(response, contentType);
        }
        public const string JsonContentType = "application/json";
        public const string HtmlContentType = "text/html";
        public const string TextContentType = "text/plain";

        public static async IAsyncEnumerable<RestAPIResult<ResultSet>> GetResultSetAsync(string path,
                                                                                         NameValueCollection? query,
                                                                                         bool all = false)
        {
            var sb = new StringBuilder(path);
            if (query != null && query.Count > 0)
            {
                sb.Append('?');
                sb.Append(query.ToString());
            }
            string nextPathAndQuery = sb.ToString();
            RestAPIResult<ResultSet> apiResult;
            do
            {
                apiResult = await GetAsync<ResultSet>(nextPathAndQuery);
                yield return apiResult;
                nextPathAndQuery = string.IsNullOrEmpty(apiResult.Contents.Next) ? string.Empty : apiResult.Contents.Next;
            } while (all && !string.IsNullOrEmpty(nextPathAndQuery));
        }
        /// <summary>
        /// Request <see cref="HttpMethod.Get">GET</see> to AWX
        /// and deserialize the contents json to <typeparamref name="T"/>.<br/>
        /// <c>Accept</c> header will be set when:
        /// <list type="bullet"><paramref name="type"/> is <see cref="AcceptType.Json"/> => <c>application/json</c> (default)</list>
        /// <list type="bullet"><paramref name="type"/> is <see cref="AcceptType.Html"/> => <c>text/html</c></list>
        /// <list type="bullet"><paramref name="type"/> is <see cref="AcceptType.Text"/> => <c>text/plain</c></list>
        /// <br/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pathAndQuery"></param>
        /// <param name="type"></param>
        /// <returns><typeparamref name="T"/> object of deserialized from JSON</returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="RestAPIException"></exception>
        public static async Task<RestAPIResult<T>> GetAsync<T>(string pathAndQuery, AcceptType type = AcceptType.Json)
            where T : class
        {
            using HttpRequestMessage request = new(HttpMethod.Get, pathAndQuery);
            switch (type)
            {
                case AcceptType.Json:
                    request.Headers.Add("Accept", JsonContentType);
                    break;
                case AcceptType.Html:
                    request.Headers.Add("Accept", HtmlContentType);
                    break;
                case AcceptType.Text:
                    request.Headers.Add("Accept", TextContentType);
                    break;
            }
            using HttpResponseMessage response = await Client.SendAsync(request);
            return await HandleResponse<T>(response);
        }
        /// <summary>
        /// Request <see cref="HttpMethod.Get">GET</see> to AWX
        /// and deserialize the contents json to <see cref="ResultSet{T}">ResultSet&lt;<typeparamref name="T"/>&gt;</see>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="query"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="RestAPIException"></exception>
        public static async IAsyncEnumerable<RestAPIResult<ResultSet<T>>> GetResultSetAsync<T>(string path,
                                                                                               NameValueCollection? query,
                                                                                               bool all = false)
            where T : class
        {
            RestAPIResult<ResultSet<T>> apiResult;
            var sb = new StringBuilder(path);
            if (query != null && query.Count > 0)
            {
                sb.Append('?');
                sb.Append(query.ToString());
            }
            string nextPathAndQuery = sb.ToString();
            do
            {
                apiResult = await GetAsync<ResultSet<T>>(nextPathAndQuery);
                yield return apiResult;
                if (apiResult.Contents == null)
                {
                    break;
                }
                nextPathAndQuery = string.IsNullOrEmpty(apiResult.Contents.Next) ? string.Empty : apiResult.Contents.Next;
            } while (all && !string.IsNullOrEmpty(nextPathAndQuery));
        }
        /// <summary>
        /// Request <see cref="HttpMethod.Options">OPTIONS</see> to AWX
        /// for getting API help document.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="RestAPIException"></exception>
        public static async Task<RestAPIResult<T>> OptionsJsonAsync<T>(string path) where T : class
        {
            using HttpRequestMessage request = new(HttpMethod.Options, path);
            using HttpResponseMessage response = await Client.SendAsync(request);
            return await HandleResponse<T>(response);
        }
        /// <summary>
        /// Create a HTTP request content as string.
        /// Serialize as JSON when <paramref name="data"/> is object (not a string).
        /// </summary>
        /// <param name="data"></param>
        /// <returns><see cref="StringContent"/></returns>
        private static StringContent GetStringContent(object? data)
        {
            if (data == null)
            {
                return new StringContent(string.Empty);
            }
            switch (data)
            {
                case string stringData:
                    return new StringContent(stringData, Encoding.UTF8, JsonContentType);
                default:
                    string jsonString = JsonSerializer.Serialize(data, Json.DeserializeOptions);
                    return new StringContent(jsonString, Encoding.UTF8, JsonContentType);
            }
        }
        /// <summary>
        /// Request <see cref="HttpMethod.Post">POST</see> to AWX
        /// for creating resouce.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="RestAPIException"></exception>
        public static async Task<RestAPIPostResult<T>> PostJsonAsync<T>(string path, object? data) where T : class
        {
            using var jsonContent = GetStringContent(data);
            using HttpResponseMessage response = await Client.PostAsync(path, jsonContent);
            return await HandlePostResponse<T>(response);
        }
        /// <summary>
        /// Request <see cref="HttpMethod.Put">PUT</see> to AWX
        /// for replacing the whole of the resouce.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="RestAPIException"></exception>
        public static async Task<RestAPIResult<T>> PutJsonAsync<T>(string path, object? data) where T : class
        {
            using var jsonContent = GetStringContent(data);
            using HttpResponseMessage response = await Client.PutAsync(path, jsonContent);
            return await HandleResponse<T>(response);
        }
        /// <summary>
        /// Request <see cref="HttpMethod.Patch">PATCH</see> to AWX
        /// for replacing parts of the resouce.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="RestAPIException"></exception>
        public static async Task<RestAPIResult<T>> PatchJsonAsync<T>(string path, object? data) where T : class
        {
            using var jsonContent = GetStringContent(data);
            using HttpResponseMessage response = await Client.PatchAsync(path, jsonContent);
            return await HandleResponse<T>(response);
        }
        /// <summary>
        /// Request <see cref="HttpMethod.Delete">DELETE</see> to AWX
        /// for deleting the resource.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="RestAPIException"></exception>
        public static async Task<RestAPIResult<string>> DeleteAsync(string path)
        {
            using HttpResponseMessage response = await Client.DeleteAsync(path);
            return await HandleResponse<string>(response);
        }
    }

    /// <summary>
    /// The exception from <see cref="RestAPI"/>
    /// </summary>
    public class RestAPIException : HttpRequestException
    {
        public RestAPIException(string? message, HttpResponseMessage response, Exception? inner = null)
            : base(HttpRequestError.InvalidResponse, message, inner, response.StatusCode)
        {
            Response = new RestAPIResponse(response);
        }
        public RestAPIResponse Response { get; }
    }
}
