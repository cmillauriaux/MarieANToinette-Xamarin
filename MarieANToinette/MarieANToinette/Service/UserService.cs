using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MarieANToinette.Service
{
    public class UserService
    {
        private string userId;

        public async Task<string> GetUserId()
        {
            userId = await DependencyService.Get<IFirebaseAuthentication>().GetUserId();
            return userId;
        }

        public async Task GetUserAccount()
        {
            /*if (!IsUserConnected())
            {
                await GetUserId();
            }*/
        }

        public async void UpdateUserAccount()
        {

        }

        public async Task AddPictureToFavorite(string pictureId)
        {
            var userId = await GetUserId();
            if (userId == null || userId == "")
            {
                throw new Exception("User not connected");
            }
            await DependencyService.Get<IFirebaseDatabase>().SaveData("users", new string[] { userId, "Favorites", pictureId }, true);
        }

        public async Task DeletePictureFromFavorite(string pictureId)
        {
            var userId = await GetUserId();
            if (userId == null || userId == "")
            {
                throw new Exception("User not connected");
            }
            await DependencyService.Get<IFirebaseDatabase>().DeleteData("users", new string[] { userId, "Favorites", pictureId });
        }

        public async void GetFavoritesUser()
        {

        }

        public async void GetFavoritesPicture(string pictureId)
        {

        }
    }
}
