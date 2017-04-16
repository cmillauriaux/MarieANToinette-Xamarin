using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MarieANToinette.Model;

namespace MarieANToinette.Service
{
    public class RestService
    {
        HttpClient client;

        public RestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<Picture> RefreshPictureAsync()
        {
            var uri = new Uri(string.Format(Constants.RestUrl, string.Empty));
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var picture = JsonConvert.DeserializeObject<Picture>(content);
                return picture;
            }
            else
            {
                throw new Exception("Cannot refresh picture");
            }
        }

        public async Task<Picture> NextPictureAsync(long dateTime)
        {
            var uri = new Uri(string.Format(Constants.NextPictureUrl, dateTime));
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var picture = JsonConvert.DeserializeObject<Picture>(content);
                return picture;
            }
            else
            {
                throw new Exception("Cannot refresh picture");
            }
        }

        public async Task<Picture> PreviousPictureAsync(long dateTime)
        {
            var uri = new Uri(string.Format(Constants.PreviousPictureUrl, dateTime));
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var picture = JsonConvert.DeserializeObject<Picture>(content);
                return picture;
            }
            else
            {
                throw new Exception("Cannot refresh picture");
            }
        }
    }
}
