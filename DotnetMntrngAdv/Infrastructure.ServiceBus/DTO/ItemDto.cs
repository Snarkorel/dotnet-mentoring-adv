namespace Infrastructure.ServiceBus.DTO
{
    public class ItemDto
    {
        int Id { get; set; }
        string Name { get; set; }
        string Image { get; set; }
        decimal Price { get; set; }
        int Quantity { get; set; }
    }
}
