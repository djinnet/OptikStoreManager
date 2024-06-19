namespace WebAppStoreManager.Services;

//Primary used for decontructed response json
public record Response<T>(T Result, int Httpstatus, string Status, List<object> Errors);
