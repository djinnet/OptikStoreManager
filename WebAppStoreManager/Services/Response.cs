namespace WebAppStoreManager.Services;

public record Response<T>(T Result, int Httpstatus, string Status, List<object> Errors);
