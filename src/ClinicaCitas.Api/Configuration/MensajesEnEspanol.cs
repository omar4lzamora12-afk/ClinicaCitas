using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace ClinicaCitas.Api.Configuration;

/// <summary>
/// Traduce al español los mensajes que ASP.NET Core genera por sí solo cuando no puede convertir
/// un valor (por ejemplo, <c>GET /api/v1/citas/abc</c>), para mantener respuestas claras.
/// </summary>
public static class MensajesEnEspanol
{
    public static void Aplicar(DefaultModelBindingMessageProvider mensajes)
    {
        mensajes.SetValueMustNotBeNullAccessor(campo => $"El campo '{campo}' es obligatorio.");
        mensajes.SetMissingBindRequiredValueAccessor(campo => $"Falta el valor obligatorio '{campo}'.");
        mensajes.SetMissingKeyOrValueAccessor(() => "Se requiere una clave y un valor.");
        mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "El cuerpo de la solicitud es obligatorio y debe ser un JSON válido.");
        mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para '{campo}'.");
        mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
        mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor proporcionado no es válido para '{campo}'.");
        mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor proporcionado no es válido.");
        mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
        mensajes.SetValueMustBeANumberAccessor(campo => $"El campo '{campo}' debe ser un número.");
        mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "El valor debe ser un número.");
    }
}
