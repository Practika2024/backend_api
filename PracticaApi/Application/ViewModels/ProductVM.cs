using Domain.Products;

namespace Application.ViewModels;
public class ProductVM
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public ProductTypeId TypeId { get; set; }

    public ProductVM(ProductEntity productEntity)
    {
        Id = productEntity.Id.Value;
        Name = productEntity.Name;
        Description = productEntity.Description;
        ManufactureDate = productEntity.ManufactureDate;
        TypeId = productEntity.Type.Id;
    }
}