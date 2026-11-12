# API de Citas Médicas – Clínica Privada

API RESTful desarrollada con **C# y .NET Core** para registrar, consultar, actualizar y cancelar las citas médicas de una clínica privada. Los datos se guardan **en memoria** (listas `List<T>`), sin base de datos, y se reinician cada vez que se detiene la aplicación.

Curso: Desarrollo de Servicios Web – Docente: Ing. Jhorlly Vela

## Equipo

| Integrante | Rol | Responsabilidades |
|---|---|---|
| Gabriel Omar Alzamora Yarleque | Líder técnico / Desarrollador backend | Servicios (lógica de negocio), controladores, `Program.cs`, integración del proyecto, repositorio GitHub y README |
| Jhonny Bogado Villegas | Desarrollador de modelo de datos | Entidades, DTOs con validaciones (`DataAnnotations`) y datos de prueba iniciales |
| Alan Agreda Aredo | QA y manejo de errores | Middleware de excepciones, pruebas funcionales en Postman, capturas y documentación del informe |

## Requisitos

- [.NET SDK](https://dotnet.microsoft.com/es-es/download)
- Visual Studio Code con la extensión C# Dev Kit
- Postman
- Git

## Instalación y ejecución

```bash
git clone https://github.com/omar4lzamora12-afk/ClinicaCitas.git
cd ClinicaCitasApi
code .
dotnet build
dotnet run
```

La API queda escuchando en **http://localhost:5080**.

## Estructura del proyecto

```
ClinicaCitasApi/
├── Controllers/            Reciben las peticiones HTTP
│   ├── CitasController.cs
│   ├── PacientesController.cs
│   └── MedicosController.cs
├── Entidades/              Clases del dominio
│   ├── Cita.cs
│   ├── Paciente.cs
│   └── Medico.cs
├── Dtos/                   Datos de entrada con validaciones
│   ├── CitaDto.cs
│   ├── PacienteDto.cs
│   └── MedicoDto.cs
├── Servicios/              Interfaces y lógica de negocio (listas en memoria)
│   ├── ICitaService.cs / CitaService.cs
│   ├── IPacienteService.cs / PacienteService.cs
│   └── IMedicoService.cs / MedicoService.cs
├── Errores/                Manejo centralizado de excepciones
│   ├── MiddlewareExcepciones.cs
│   ├── RespuestaError.cs
│   ├── NotFoundException.cs
│   └── ConflictException.cs
├── Postman/                Colección de pruebas
├── Program.cs
└── README.md
```

## Endpoints

### Citas (CRUD completo)

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/citas` | Lista todas las citas | 200 |
| GET | `/api/citas/{id}` | Consulta una cita por Id | 200, 404 |
| POST | `/api/citas` | Registra una nueva cita | 201, 400, 404, 409 |
| PUT | `/api/citas/{id}` | Actualiza una cita | 200, 400, 404, 409 |
| PATCH | `/api/citas/{id}/cancelar` | Cancela la cita (estado `Cancelada`) | 200, 404, 409 |
| DELETE | `/api/citas/{id}` | Elimina la cita del listado | 204, 404 |

### Pacientes y Médicos

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/pacientes` | Lista pacientes |
| GET | `/api/pacientes/{id}` | Consulta un paciente |
| POST | `/api/pacientes` | Registra un paciente |
| GET | `/api/medicos` | Lista médicos |
| GET | `/api/medicos/{id}` | Consulta un médico |
| POST | `/api/medicos` | Registra un médico |

Datos iniciales: pacientes con Id 1 y 2, médicos con Id 1, 2 y 3.

### Ejemplos de cuerpo (JSON)

**POST / PUT `/api/citas`**
```json
{
  "pacienteId": 1,
  "medicoId": 1,
  "fechaHora": "2027-03-15T10:00:00",
  "motivo": "Control de presion arterial"
}
```

**POST `/api/pacientes`**
```json
{
  "nombre": "Ana Torres",
  "dni": "45678912",
  "email": "ana.torres@correo.com",
  "telefono": "956781234"
}
```

**POST `/api/medicos`**
```json
{
  "nombre": "Dra. Rosa Medina",
  "especialidad": "Dermatologia"
}
```

## Validaciones
- **Cita:** `PacienteId` y `MedicoId` válidos (`[Range]`), `FechaHora` obligatoria (`[Required]`), `Motivo` de 5 a 200 caracteres (`[Required]`, `[StringLength]`).
- **Paciente:** nombre 2–50 caracteres, DNI de 8 dígitos (`[RegularExpression]`), email válido (`[EmailAddress]`), teléfono de 9 dígitos que empieza con 9.
- **Médico:** nombre 2–50 caracteres, especialidad 3–50 caracteres.

## Reglas de negocio

- El paciente y el médico de la cita deben existir (404 si no).
- La fecha y hora de la cita debe ser posterior a la actual (400).
- Un médico no puede tener dos citas programadas en la misma fecha y hora (409).
- No se puede modificar ni volver a cancelar una cita ya cancelada (409).
- No se pueden registrar dos pacientes con el mismo DNI (409).

## Manejo de errores

Todos los errores no controlados son capturados por `MiddlewareExcepciones` y devueltos en un JSON estandarizado:

```json
{
  "CodigoEstado": 404,
  "Mensaje": "El codigo de Cita: 99 ingresado no Existe, Verifique!!",
  "Ruta": "/api/citas/99"
}
```

| Código | Cuándo ocurre |
|---|---|
| 400 | Regla de entrada inválida (ej. fecha pasada). Los errores de `DataAnnotations` los devuelve `[ApiController]` automáticamente con código 400 |
| 404 | La cita, el paciente o el médico no existen |
| 409 | Conflicto de horario, DNI duplicado o cita ya cancelada |
| 500 | Error inesperado del servidor (el detalle solo se registra en el log) |

## Pruebas con Postman

1. Ejecutar la API con `dotnet run`.
2. En Postman: **Import** → seleccionar `Postman/ClinicaCitasApi.postman_collection.json`.
3. Ejecutar las peticiones en orden. Cada una incluye una prueba que verifica el código de estado esperado.
4. Si Postman marca error de certificado, desactivar *SSL certificate verification* (ver guía de instaladores).

## Subir el proyecto a GitHub

```bash
git init
git add .
git commit -m "API de citas medicas"
git branch -M main
git remote add origin https://github.com/<usuario>/ClinicaCitasApi.git
git push -u origin main
```