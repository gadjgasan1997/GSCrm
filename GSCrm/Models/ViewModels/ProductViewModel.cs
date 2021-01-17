namespace GSCrm.Models.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }

        public string SearchName { get; set; }
        public string MinConst { get; set; }
        public string MaxConst { get; set; }
    }
}
