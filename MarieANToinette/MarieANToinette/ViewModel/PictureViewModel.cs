using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MarieANToinette.Model;
using MarieANToinette.Service;
using Xamarin.Forms;

namespace MarieANToinette
{
    public class PictureViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        RestService service = new RestService();
        UserService userSvc = new UserService();

        Picture picture;
        string pictureUrl;
        string duree;
        string title = "Marie-ANToinette";
        bool isFavorite;

        public ICommand RefreshCommand { protected set; get; }
        public ICommand ChangeCommand { protected set; get; }
        public ICommand SetFavoriteCommand { protected set; get; }

        public PictureViewModel()
        {
            this.Picture = new Picture();
            this.registerCommands();
            RefreshPicture();
        }

        private void registerCommands()
        {
            this.RefreshCommand = new Command<string>((key) =>
            {
                this.RefreshPicture();
            });
            this.ChangeCommand = new Command<string>((key) =>
            {
                if (key == "previous")
                {
                    this.PreviousPicture();
                }
                else
                {
                    this.NextPicture();
                }
            });
            this.SetFavoriteCommand = new Command<string>(async (key) =>
            {
                if (IsFavorite)
                {
                    await RemovePictureFromFavorite();
                } else
                {
                    await AddPictureToFavorite();
                }
            });
        }

        public bool IsFavorite
        {
            set
            {
                if (isFavorite != value)
                {
                    isFavorite = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("IsFavorite"));
                    }
                }
            }
            get
            {
                return isFavorite;
            }
        }

        public string Title
        {
            set
            {
                if (title != value)
                {
                    title = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("Title"));
                    }
                }
            }
            get
            {
                return title;
            }
        }

        public Picture Picture
        {
            set
            {
                if (picture != value)
                {
                    
                    if (value != null)
                    {
                        picture = value;
                        Title = "Image du " + String.Format("{0:dd/MM/yyyy à HH:mm}", UnixTimeStampToDateTime(picture.DateTime));
                    }

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("Picture"));
                    }
                }
            }
            get
            {
                return picture;
            }
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public string PictureUrl
        {
            set
            {
                if (PictureUrl != value)
                {
                    pictureUrl = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("PictureUrl"));
                    }
                }
            }
            get
            {
                return pictureUrl;
            }
        }

        public string Duree
        {
            set
            {
                if (Duree != value)
                {
                    duree = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("Duree"));
                    }
                }
            }
            get
            {
                return duree;
            }
        }

        public async void RefreshPicture()
        {
            Picture = await service.RefreshPictureAsync();
            if (Picture != null)
            {
                PictureUrl = "https://storage.googleapis.com/ants-photos/" + Picture.FileName;
            }
        }

        public async void NextPicture()
        {
            Picture = await service.NextPictureAsync(picture.DateTime + getDelta());
            if (Picture != null)
            {
                PictureUrl = "https://storage.googleapis.com/ants-photos/" + Picture.FileName;
            }
        }

        public async void PreviousPicture()
        {
            Picture = await service.PreviousPictureAsync(picture.DateTime - getDelta());
            if (Picture != null)
            {
                PictureUrl = "https://storage.googleapis.com/ants-photos/" + Picture.FileName;
            }
        }

        private long getDelta()
        {
            if (this.Duree == "")
            {
                return 0;
            }
            switch (this.Duree)
            {
                case "1 Minute":
                    return 0;
                case "15 Minutes":
                    return 15 * 60;
                case "1 Heure":
                    return 60 * 60;
                case "6 Heures":
                    return 6 * 60 * 60;
                case "1 jour":
                    return 24 * 60 * 60;
                case "1 semaine":
                    return 7 * 24 * 60 * 60;
                case "1 mois":
                    return 30 * 24 * 60 * 60;
            }
            return 0;
        }

        public async Task AddPictureToFavorite()
        {
            if (Picture != null && Picture.FileName != null && Picture.FileName != "")
            {
                IsFavorite = true;
                await userSvc.AddPictureToFavorite(Picture.FileName);
            }
        }

        public async Task RemovePictureFromFavorite()
        {
            if (Picture != null && Picture.FileName != null && Picture.FileName != "")
            {
                IsFavorite = false;
                await userSvc.DeletePictureFromFavorite(Picture.FileName);
            }
        }
    }
}
