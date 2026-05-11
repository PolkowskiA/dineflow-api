namespace DineFlow.Application.Common.Products
{
    public interface IProductService
    {
        ProductDto GetById(Guid id);
    }
}