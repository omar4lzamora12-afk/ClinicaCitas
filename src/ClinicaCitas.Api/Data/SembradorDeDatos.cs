using ClinicaCitas.Api.Models.Entities;
using ClinicaCitas.Api.Repositories;

namespace ClinicaCitas.Api.Data;

/// <summary>
/// Carga datos de ejemplo al iniciar la aplicación, para que la API no arranque vacía y se pueda
/// probar de inmediato. Como la información vive en memoria, se vuelve a cargar en cada arranque.
/// Las fechas de las citas se calculan a partir de HOY, así nunca quedan en el pasado.
/// </summary>
public sealed class SembradorDeDatos
{
    private readonly IPacienteRepository _pacientes;
    private readonly IMedicoRepository _medicos;
    private readonly ICitaRepository _citas;
    private readonly TimeProvider _reloj;
    private readonly ILogger<SembradorDeDatos> _logger;

    public SembradorDeDatos(IPacienteRepository pacientes, IMedicoRepository medicos, ICitaRepository citas,
                            TimeProvider reloj, ILogger<SembradorDeDatos> logger)
    {
        _pacientes = pacientes;
        _medicos = medicos;
        _citas = citas;
        _reloj = reloj;
        _logger = logger;
    }

    public void Sembrar()
    {
        if (_pacientes.Contar() > 0 || _medicos.Contar() > 0)
        {
            return; // ya hay datos: no se duplica nada
        }

        var lucia = _pacientes.Agregar(new Paciente("Lucía", "Fernández Rojas", "45871236", "987654321",
            "lucia.fernandez@correo.com", new DateOnly(1990, 5, 14), "Av. Javier Prado Este 1200, San Isidro"));
        var carlos = _pacientes.Agregar(new Paciente("Carlos", "Mendoza Quispe", "40123987", "912345678",
            "carlos.mendoza@correo.com", new DateOnly(1985, 11, 2), "Jr. Cusco 450, Lima"));
        var ana = _pacientes.Agregar(new Paciente("Ana", "Torres Vega", "72654318", "998877665",
            "ana.torres@correo.com", new DateOnly(2001, 3, 27)));

        var salazar = _medicos.Agregar(new Medico("Ricardo", "Salazar Paredes", "08123456", "955112233",
            "ricardo.salazar@clinica.pe", "045821", "Medicina General"));
        var villanueva = _medicos.Agregar(new Medico("Patricia", "Villanueva León", "09234567", "944223344",
            "patricia.villanueva@clinica.pe", "051234", "Pediatría"));
        _medicos.Agregar(new Medico("Jorge", "Ramírez Cueva", "10345678", "933334455",
            "jorge.ramirez@clinica.pe", "038765", "Cardiología"));

        var manana = ProximoDiaHabil(1);
        var pasadoManana = ProximoDiaHabil(2);

        _citas.Agregar(new Cita(lucia.Id, salazar.Id, manana.AddHours(9), 30, "Control general anual"));
        _citas.Agregar(new Cita(carlos.Id, salazar.Id, manana.AddHours(10), 30, "Dolor de espalda persistente"));
        _citas.Agregar(new Cita(ana.Id, villanueva.Id, pasadoManana.AddHours(11), 45,
            "Control de crecimiento", "Traer carné de vacunación"));

        _logger.LogInformation("Datos de prueba cargados: {Pacientes} pacientes, {Medicos} médicos, {Citas} citas.",
            _pacientes.Contar(), _medicos.Contar(), _citas.Contar());
    }

    /// <summary>Medianoche del día indicado a partir de hoy, saltando los domingos (la clínica no atiende).</summary>
    private DateTime ProximoDiaHabil(int diasDesdeHoy)
    {
        var fecha = _reloj.GetLocalNow().Date.AddDays(diasDesdeHoy);
        while (fecha.DayOfWeek == DayOfWeek.Sunday)
        {
            fecha = fecha.AddDays(1);
        }
        return fecha;
    }
}
