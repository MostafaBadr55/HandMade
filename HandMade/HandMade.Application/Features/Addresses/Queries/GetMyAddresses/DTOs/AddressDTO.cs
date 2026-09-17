namespace HandMade.Application.Features.Addresses.Queries.GetMyAddresses.DTOs
{
    public class AddressDTO
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string DetailedAddress { get; set; }
        public bool IsDefault { get; set; }
    }
}
