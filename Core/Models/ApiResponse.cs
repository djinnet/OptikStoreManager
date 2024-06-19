using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace Core.Models;

/// <summary>
/// This is the API response that contains a result, a status and a httpstatus/code along with the list of errors.
/// </summary>
/// <typeparam name="T">The result</typeparam>
/// <typeparam name="E">The status</typeparam>
public class ApiResponse<T, E>
{
    public ApiResponse(T result, E status)
    {
        Result = result;
        Status = status;
    }

    public ApiResponse()
    {
    }

    public T Result { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public E Status { get; set; }

    public int HttpStatus { get; set; }

    public List<string> Errors { get; set; } = [];
    public bool AddModelErrors(ModelStateDictionary modelState)
    {
        Errors = modelState.Root.Children.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
        return Errors.Count > 0;
    }

    public bool AddModelErrors(string errormsg)
    {
        Errors.Add(errormsg);
        return Errors.Count > 0;
    }
}
