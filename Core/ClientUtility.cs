using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Greatbone.Core
{
    public static class ClientUtility
    {
        public static string GetValue(this HttpHeaders headers, string name)
        {
            if (headers.TryGetValues(name, out var values))
            {
                string[] strs = values as string[];
                return strs?[0];
            }
            return null;
        }

        public static void CallAny(this Client[] df)
        {
        }

        public static void CallAll(this Client[] df)
        {
        }

        public static async void CallAll(Task<HttpResponseMessage>[] requests, Action<HttpResponseMessage> a)
        {
            HttpResponseMessage[] results = await Task.WhenAll(requests);
            for (int i = 0; i < results.Length; i++)
            {
                a(results[i]);
            }
        }
    }
}