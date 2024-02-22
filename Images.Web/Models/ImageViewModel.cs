using Images.Data;

namespace Images.Web.Models
{
    public class ImageViewModel
    {
        public Image Image { get; set; }
        public bool NeedPassword { get; set; }
        public bool InvalidPassword { get; set; }
    }
}
