using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoSpaceApp.Models
{
    public class NewsModel : ModelBase
    {
        private String _id;
        public String Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }

        private String _title;
        public String Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged("Title"); }
        }

        private String _synopsis;
        public String Synopsis
        {
            get { return _synopsis; }
            set { _synopsis = value; NotifyPropertyChanged("Synopsis"); }
        }

        private String _date;
        public String Date
        {
            get { return _date; }
            set { _date = value; NotifyPropertyChanged("Date"); }
        }

        private String _image;
        public String Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

        private String _largeImage;
        public String LargeImage
        {
            get { return _largeImage; }
            set { _largeImage = value; NotifyPropertyChanged("LargeImage"); }
        }

        private String _content;
        public String Content
        {
            get { return _content; }
            set { _content = value; NotifyPropertyChanged("Content"); }
        }
    }
}
