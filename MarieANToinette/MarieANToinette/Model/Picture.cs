using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarieANToinette.Model
{
    public class Picture
    {
        public string FileName { get; set; }
        public string CameraID { get; set; }
        public long DateTime { get; set; }
        public string Link { get; set; }
        public string Next { get; set; }
    }
}
