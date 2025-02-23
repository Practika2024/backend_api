namespace Domain.ProductTypeModels;

public class UpdateProductTypeModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ModifiedBy { get; set; }
}