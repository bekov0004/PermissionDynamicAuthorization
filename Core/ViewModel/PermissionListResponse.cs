namespace Core.ViewModel;

public class PermissionListResponse
{ 
    public string Type { get; set; }
    public string Value { get; set; }

    public PermissionListResponse(string type, string value)
    {
        Type = type;
        Value = value;
    }
}