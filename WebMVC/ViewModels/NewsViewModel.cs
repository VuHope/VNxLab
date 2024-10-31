using Models.Entity;

namespace WebMVC.ViewModels
{
    public class NewsViewModel
    {
        public News News { get; set; }

        public List<News> ListNews { get; set; }
    }
}
