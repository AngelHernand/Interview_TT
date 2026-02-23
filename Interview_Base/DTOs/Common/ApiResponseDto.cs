namespace Interview_Base.DTOs.Common;

//Usado como modelo estandar para todas las respuestas de la API.
public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    //Helpers estáticos
    public static ApiResponseDto<T> Ok(T data, string message = "Operación exitosa")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponseDto<T> Fail(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors ?? new() };

    public static ApiResponseDto<T> Fail(List<string> errors)
        => new() { Success = false, Message = "Error de validación", Errors = errors };
}
