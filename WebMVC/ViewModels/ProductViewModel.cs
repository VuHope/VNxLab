using Models.Entity;

namespace WebMVC.ViewModels
{
    public class ProductViewModel
    {
        public ResearchProduct ResearchProduct { get; set; }
        public List<ResearchProduct> ListResearchProduct { get; set; }
        public List<ProductImage> ProductImages { get; set; }
    }
}
