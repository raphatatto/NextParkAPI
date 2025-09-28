using System.Collections.Generic;

namespace NextParkAPI.Models.Responses
{
    public class ResourceResponse<T>
    {
        public ResourceResponse(T data)
        {
            Data = data;
            Links = new List<Link>();
        }

        public T Data { get; set; }
        public List<Link> Links { get; set; }
    }
}
