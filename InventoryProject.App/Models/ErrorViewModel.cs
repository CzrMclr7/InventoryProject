namespace InventoryProject.App.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
    }
}
