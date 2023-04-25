namespace Infrastructure.ServiceBus.DTO
{
    public class ItemDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Image { get; init; }
        public decimal Price { get; init; }
        public int Quantity { get; init; }
    }
}
