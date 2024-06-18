using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Core.Models;
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
