namespace PatientDemo.Application.Exceptions;

/// <summary>
/// Исключение, если объект не найден
/// </summary>
public class NotFoundException(string message) : Exception(message);