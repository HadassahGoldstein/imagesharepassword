using imagesharepassword.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace imagesharepassword.Web.Models
{
    public class ImageAddedViewModel
    {
        public int ImageId { get; set; }
        public string Password { get; set; }
    }
    public class ShowImageViewModel
    {
        public Image Image { get; set; }
        public bool ShowImage { get; set; }
        public int Id { get; set; }
        public bool FalsePassword { get; set; }
        public List<int> Ids { get; set; }
    }
}
