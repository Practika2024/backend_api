using Domain.Products;

namespace Application.ViewModels;
public class ProductVM
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public ProductType Type { get; set; }

    public ProductVM(Product product)
    {
        Id = product.Id.Value;
        Name = product.Name;
        Description = product.Description;
        ManufactureDate = product.ManufactureDate;
        Type = product.Type;
    }
}