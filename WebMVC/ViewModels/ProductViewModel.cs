using Models.Entity;

namespace WebMVC.ViewModels
{
    public class ProductViewModel
    {
        public ResearchProduct ResearchProduct { get; set; }
        public List<ResearchProduct> ListResearchProduct { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
    }
}
